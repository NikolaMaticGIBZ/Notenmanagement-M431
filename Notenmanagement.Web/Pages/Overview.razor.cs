using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Notenmanagement.Web.Pages;

/// <summary>
/// Code behind file for overview
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Overview : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    private const string up = "up";
    private const string down = "down";
    private const string normal = "normal";

    private string Role { get; set; } = string.Empty;
    private bool IsAccessDenied { get; set; } = false;

    private bool IsDark;

    private List<(int Id, string Name)> Rektoren = new()
    {
        (1, "Regula Tobler"),
        (2, "Werner Odermatt"),
        (3, "Patrick Zeiger"),
        (4, "Alex Kobel")
    };

    private GradeFormModel gradeForm = new();
    private string RoundingOption { get; set; } = normal;
    private string RektorValidationMessage { get; set; } = string.Empty;

    // ================= TEACHER STATE =================
    private string teacherTab = "new"; // "new" | "mine"
    private string myTab = "pending";  // "pending" | "approved" | "rejected"

    private List<GradeResponse> MyGrades = new();
    private List<GradeResponse> MyPending => MyGrades.Where(x => x.Status == "pending").ToList();
    private List<GradeResponse> MyApproved => MyGrades.Where(x => x.Status == "approved").ToList();
    private List<GradeResponse> MyRejected => MyGrades.Where(x => x.Status == "rejected").ToList();

    // teacher edit modal
    private bool showEditModal = false;
    private GradeResponse? editGrade;
    private UpdateGradeRequest editForm = new();
    private string editError = "";

    // ================= REKTOR STATE =================
    private string activeTab = "pending"; // wird im rektor-Markup benutzt

    private List<GradeResponse> PendingGrades = new();
    private List<GradeResponse> ApprovedGrades = new();
    private List<GradeResponse> RejectedGrades = new();

    // rektor decision modal (NEU)
    private bool showModal = false;
    private string modalType = "";     // "approved" | "rejected"
    private int selectedGradeId;
    private string decisionNote = "";

    private void SetRounding(string option) => RoundingOption = option;
    private bool IsSelected(string option) => RoundingOption == option;
    private decimal RoundedGrade => ApplyRounding(gradeForm.GradeValue, RoundingOption);

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
        {
            await JS.InvokeVoidAsync("localStorage.removeItem", "jwt");
            NavManager.NavigateTo("/");
            return;
        }

        Role = GetRoleFromToken(token);

        if (Role != "teacher" && Role != "rektor")
        {
            IsAccessDenied = true;
            return;
        }

        var theme = await JS.InvokeAsync<string>("theme.get");
        IsDark = theme == "dark";

        if (Role == "teacher")
        {
            await LoadMyGrades();
            teacherTab = "mine";
        }
        else if (Role == "rektor")
        {
            await LoadGrades();          // NEU: wichtig für rektor-Listen
            activeTab = "pending";       // optional, aber sinnvoll als default
        }
    }
    private decimal ApplyRounding(decimal grade, string rounding)
    {
        grade = Math.Clamp(grade, 1m, 6m);
        return rounding switch
        {
            "up" => Math.Ceiling(grade * 2) / 2,
            "down" => Math.Floor(grade * 2) / 2,
            _ => Math.Round(grade * 2, MidpointRounding.AwayFromZero) / 2
        };
    }
    private async Task ToggleTheme(ChangeEventArgs e)
    {
        IsDark = (bool)e.Value!;
        await JS.InvokeVoidAsync("theme.set", IsDark ? "dark" : "light");
    }
    private async Task OpenMyRequests()
    {
        teacherTab = "mine";
        await LoadMyGrades();
    }

    // ================= TEACHER LOAD =================
    private async Task LoadMyGrades()
    {
        var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (string.IsNullOrEmpty(token)) return;

        Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        MyGrades = await Http.GetFromJsonAsync<List<GradeResponse>>("api/grades/mine") ?? new();
    }

    // ================= REKTOR LOAD (NEU) =================
    private async Task LoadGrades()
    {
        var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (string.IsNullOrEmpty(token)) return;

        Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        PendingGrades = await Http.GetFromJsonAsync<List<GradeResponse>>("api/grades?status=pending") ?? new();
        ApprovedGrades = await Http.GetFromJsonAsync<List<GradeResponse>>("api/grades?status=approved") ?? new();
        RejectedGrades = await Http.GetFromJsonAsync<List<GradeResponse>>("api/grades?status=rejected") ?? new();
    }

    // ================= TEACHER CREATE =================
    private async Task ConfirmAction()
    {
        RektorValidationMessage = string.Empty;
        if (gradeForm.RektorId == 0)
        {
            RektorValidationMessage = "Bitte einen Prorektor auswählen.";
            return;
        }

        try
        {
            var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
            if (string.IsNullOrEmpty(token))
            {
                await JS.InvokeVoidAsync("alert", "Sie sind nicht eingeloggt.");
                return;
            }

            var request = new CreateGradeRequest
            {
                ModuleName = gradeForm.ModuleName,
                CourseName = gradeForm.CourseName,
                StudentName = gradeForm.StudentName,
                GradeValue = RoundedGrade,
                RektorId = gradeForm.RektorId,
                Comment = gradeForm.Comment
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/grades")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Http.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                await JS.InvokeVoidAsync("eval", "document.getElementById('success-alert').classList.remove('d-none')");
                gradeForm = new();
                RoundingOption = normal;

                await LoadMyGrades();
                teacherTab = "mine";
                myTab = "pending";
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync();
                await JS.InvokeVoidAsync("alert", $"Fehler beim Senden: {errorText}");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("alert", $"Fehler: {ex.Message}");
        }
    }

    // ================= TEACHER EDIT MODAL =================
    private void OpenEditModal(GradeResponse g)
    {
        editError = "";
        editGrade = g;

        editForm = new UpdateGradeRequest
        {
            ModuleName = g.ModuleName,
            CourseName = g.CourseName,
            StudentName = g.StudentName,
            GradeValue = g.GradeValue,
            RektorId = g.RektorId,
            Comment = g.Comment ?? ""
        };

        showEditModal = true;
    }

    private void CloseEditModal()
    {
        showEditModal = false;
        editGrade = null;
        editError = "";
    }
    private async Task SaveEdit()
    {
        if (editGrade == null) return;

        editError = "";

        // ===== Validierung =====
        if (string.IsNullOrWhiteSpace(editForm.ModuleName))
        {
            editError = "Modul ist erforderlich.";
            return;
        }
        if (string.IsNullOrWhiteSpace(editForm.CourseName))
        {
            editError = "Klasse ist erforderlich.";
            return;
        }
        if (string.IsNullOrWhiteSpace(editForm.StudentName))
        {
            editError = "Schülername ist erforderlich.";
            return;
        }
        if (editForm.GradeValue < 1 || editForm.GradeValue > 6)
        {
            editError = "Die Note muss zwischen 1.0 und 6.0 liegen.";
            return;
        }
        if (editForm.RektorId == 0)
        {
            editError = "Bitte einen Rektor auswählen.";
            return;
        }

        try
        {
            var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
            if (string.IsNullOrEmpty(token)) return;

            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await Http.PutAsJsonAsync($"api/grades/{editGrade.Id}", editForm);

            if (!res.IsSuccessStatusCode)
            {
                var txt = await res.Content.ReadAsStringAsync();
                editError = txt;
                return;
            }

            showEditModal = false;
            await LoadMyGrades();
            myTab = "pending";
        }
        catch (Exception ex)
        {
            editError = ex.Message;
        }
    }
    private async Task DeleteGrade()
    {
        if (editGrade == null) return;

        bool confirm = await JS.InvokeAsync<bool>("confirm", "Möchten Sie diese offene Anfrage wirklich löschen?");
        if (!confirm) return;

        try
        {
            var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
            if (string.IsNullOrEmpty(token)) return;

            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await Http.DeleteAsync($"api/grades/mine/{editGrade.Id}");
            if (res.IsSuccessStatusCode)
            {
                showEditModal = false;
                await LoadMyGrades();
                myTab = "pending";
            }
            else
            {
                var txt = await res.Content.ReadAsStringAsync();
                editError = $"Fehler beim Löschen: {txt}";
            }
        }
        catch (Exception ex)
        {
            editError = $"Fehler: {ex.Message}";
        }
    }

    // ================= REKTOR DECISION MODAL (NEU) =================
    private void OpenModal(string type, int gradeId)
    {
        modalType = type;           // "approved" oder "rejected"
        selectedGradeId = gradeId;
        decisionNote = "";
        showModal = true;
    }
    private void CloseModal() => showModal = false;
    private async Task ConfirmProrektorAction()
    {
        var token = await JS.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (string.IsNullOrEmpty(token)) return;

        Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new { status = modalType, decisionNote = decisionNote };

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/grades/{selectedGradeId}/decision")
        {
            Content = JsonContent.Create(body)
        };

        var res = await Http.SendAsync(request);
        showModal = false;

        if (res.IsSuccessStatusCode)
        {
            await LoadGrades();
        }
        else
        {
            var txt = await res.Content.ReadAsStringAsync();
            await JS.InvokeVoidAsync("alert", $"Fehler: {txt}");
        }
    }
    private void OpenAIHelper()
    {
        if (string.IsNullOrWhiteSpace(gradeForm.StudentName) ||
            string.IsNullOrWhiteSpace(gradeForm.ModuleName) ||
            string.IsNullOrWhiteSpace(gradeForm.CourseName))
        {
            JS.InvokeVoidAsync("alert", "Bitte zuerst Schüler, Modul und Klasse ausfüllen.");
            return;
        }

        var prompt = $"Ich bin Lehrer an der Berufsschule GIBZ Kanton Zug.\n" +
                     $"Ich möchte für meinen Schüler {gradeForm.StudentName} in der Klasse {gradeForm.CourseName} " +
                     $"im Modul {gradeForm.ModuleName} eine Note von {gradeForm.GradeValue} eingeben.\n\n" +
                     "Bitte analysiere professionell, ob die Note mathematisch gerundet, auf- oder abgerundet werden sollte, " +
                     "unter Berücksichtigung der üblichen schulischen Bewertungsrichtlinien.\n" +
                     "Gib eine klare Empfehlung (Aufrunden/Abrunden/Mathematisch) und eine kurze Begründung.\n\n" +
                     "Antworte so, dass ein Lehrer direkt entscheiden kann.";

        var chatUrl = $"https://chat.openai.com/?prompt={Uri.EscapeDataString(prompt)}";
        JS.InvokeVoidAsync("window.open", chatUrl, "_blank");
    }
    private async Task Logout()
    {
        await JS.InvokeVoidAsync("localStorage.removeItem", "jwt");
        await JS.InvokeVoidAsync("sessionStorage.clear");
        NavManager.NavigateTo("/", true);
    }
    private string GetRoleFromToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return string.Empty;
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            while (payload.Length % 4 != 0) payload += "=";

            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var doc = System.Text.Json.JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("role", out var roleProp))
                return roleProp.GetString()?.ToLower() ?? string.Empty;

            if (doc.RootElement.TryGetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var claimRole))
                return claimRole.GetString()?.ToLower() ?? string.Empty;
        }
        catch { }
        return string.Empty;
    }
    private bool IsTokenExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            while (payload.Length % 4 != 0) payload += "=";

            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var exp = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("exp").GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(exp) < DateTimeOffset.UtcNow;
        }
        catch { return true; }
    }
}