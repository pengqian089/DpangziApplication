using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Dpz.Client.Pages;

public partial class Music
{
[Inject]private IJSRuntime JsRuntime { get; set; }
    
    private IJSObjectReference _module;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", 
                "./Pages/Music.razor.js");
        }
    }
}