using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Notenmanagement.Web.Pages;

/// <summary>
/// Code behind class of register 
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Register : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    private string Username { get; set; } = string.Empty;
    private string Email { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private string ErrorMessage { get; set; } = string.Empty;

    private bool IsDark { get; set; } = false;

    private string SuccessMessage { get; set; } = string.Empty;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load theme from localStorage
        string theme = await JS.InvokeAsync<string>("theme.get");
        IsDark = theme == "dark";

        if (await AuthHelper.IsLoggedIn(JS))
        {
            NavManager.NavigateTo("/overview", true);
        }
    }
    private async Task ToggleTheme(ChangeEventArgs e)
    {
        IsDark = (bool)e.Value!;
        await JS.InvokeVoidAsync("theme.set", IsDark ? "dark" : "light");
    }

    private async Task HandleRegister()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        string email = Email.ToLower().Trim();

        if (!email.EndsWith("@gibz.ch") && !email.EndsWith("@zg.ch") && !email.EndsWith("@hotmail.com") && !email.EndsWith("@gmail.com"))
        {
            ErrorMessage = "Bitte verwenden Sie eine gültige Email-Adresse.";
            return;
        }

        Shared.DTOs.RegisterRequest request = new Shared.DTOs.RegisterRequest
        {
            Username = Username,
            Email = Email,
            Password = Password
        };

        try
        {
            HttpResponseMessage response = await Http.PostAsJsonAsync("api/auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Ihr Account wurde erfolgreich erstellt! Sie werden gleich zur Login-Seite weitergeleitet.";
                StateHasChanged();

                await Task.Delay(2500);
                NavManager.NavigateTo("/");
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
        }
        catch
        {
            ErrorMessage = "Registration failed! Try again.";
        }
    }
}