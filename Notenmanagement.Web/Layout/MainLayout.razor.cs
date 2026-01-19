using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Notenmanagement.Web.Layout;

/// <summary>
/// Code behind class of the main layout
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class MainLayout : LayoutComponentBase
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// Wird nach dem Rendern der Komponente aufgerufen. 
    /// Führt beim ersten Rendern JavaScript-Code aus, um das Theme zu initialisieren.
    /// </summary>
    /// <param name="firstRender">Gibt an ob dies der erste Rendervorgang der Komponente ist.</param>
    /// <returns>Ein async Task.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeAsync<string>("theme.init");
    }
}