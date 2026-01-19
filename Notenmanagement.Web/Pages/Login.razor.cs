using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTOs;
using System.Net.Http.Json;

namespace Notenmanagement.Web.Pages;

/// <summary>
/// Code behind class of Login
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Login : ComponentBase
{
    [Inject] protected HttpClient Http { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    private string Email { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private string ErrorMessage { get; set; } = string.Empty;

    private bool IsDark { get; set; } = false;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load theme from localStorage
        var theme = await JS.InvokeAsync<string>("theme.get");
        IsDark = theme == "dark";

        // Redirect if already logged in
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

    private async Task HandleLogin()
    {
        ErrorMessage = string.Empty;

        var request = new Shared.DTOs.LoginRequest
        {
            Email = Email,
            Password = Password
        };

        try
        {
            var response = await Http.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // Temporäres 2FA-Flag setzen
                await JS.InvokeVoidAsync("sessionStorage.setItem", "requires2FA", "true");
                await JS.InvokeVoidAsync("sessionStorage.setItem", "userId", result!.UserId.ToString());

                // Weiterleiten zur 2FA-Seite
                NavManager.NavigateTo("/twofactor");
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
        }
        catch
        {
            ErrorMessage = "Login fehlgeschlagen! Bitte versuchen Sie es erneut.";
        }
    }
}