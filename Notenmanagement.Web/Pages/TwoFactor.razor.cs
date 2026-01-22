using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTOs;
using System.Net.Http.Json;

namespace Notenmanagement.Web.Pages;

/// <summary>
/// Code behind class for two factor autentication
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class TwoFactor : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    private bool IsDark { get; set; } = false;
    private bool isLoading = false;
    private string Code { get; set; } = string.Empty;
    private string ErrorMessage { get; set; } = string.Empty;
    private int UserId { get; set; }

    private bool isResending = false;
    private string ResendMessage { get; set; } = string.Empty;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load theme automatically
        var theme = await JS.InvokeAsync<string>("theme.get");
        IsDark = theme == "dark";

        // Check 2FA session
        var requires2FA = await JS.InvokeAsync<string>("sessionStorage.getItem", "requires2FA");
        var userIdString = await JS.InvokeAsync<string>("sessionStorage.getItem", "userId");

        if (string.IsNullOrEmpty(requires2FA) || string.IsNullOrEmpty(userIdString))
        {
            NavManager.NavigateTo("/");
            return;
        }

        if (!int.TryParse(userIdString, out int userId))
        {
            NavManager.NavigateTo("/");
            return;
        }

        UserId = userId;
    }
    private async Task VerifyCode()
    {
        isLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new Verify2FARequest
            {
                UserId = UserId,
                Code = Code
            };

            var response = await Http.PostAsJsonAsync("api/auth/verify-2fa", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Verify2FAResponse>();

                await JS.InvokeVoidAsync("localStorage.setItem", "jwt", result!.Token);
                await JS.InvokeVoidAsync("sessionStorage.removeItem", "requires2FA");
                await JS.InvokeVoidAsync("sessionStorage.removeItem", "userId");
                await JS.InvokeVoidAsync("sessionStorage.setItem", "userRole", result.Role);

                if (result.Role == "rektor" || result.Role == "teacher")
                    NavManager.NavigateTo("/overview");
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
    private async Task ResendCode()
    {
        isResending = true;
        ResendMessage = string.Empty;

        try
        {
            var request = new Shared.DTOs.Resend2FARequest
            {
                UserId = UserId
            };

            var response = await Http.PostAsJsonAsync("api/auth/resend-2fa", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Resend2FAResponse>();
                ResendMessage = result?.Message ?? "Code wurde erneut gesendet.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ResendMessage = $"Fehler beim Senden: {error}";
            }
        }
        catch (Exception ex)
        {
            ResendMessage = $"Fehler beim Senden: {ex.Message}";
        }
        finally
        {
            isResending = false;
        }
    }
}