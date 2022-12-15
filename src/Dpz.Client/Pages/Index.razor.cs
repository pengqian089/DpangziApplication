using System.Text.Json;
using Dpz.Client.Data;
using Dpz.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;

namespace Dpz.Client.Pages;

public partial class Index
{
    private bool _isLoading = false;

    private double _width = 0d;

    private List<ArticleMiniModel> _source = new();

    private List<PictureModel> _banner = new();
    
    private DotNetObjectReference<Index> _objectReference;

    [Inject] private ArticleService ArticleService { get; set; }
    [Inject] private CommunityService CommunityService { get; set; }
    //[Inject] private ILogger<Index> Logger { get; set; }
    [Inject] private IJSRuntime JsRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _width = await JsRuntime.InvokeAsync<double>("getWindowWidth");
        _objectReference = DotNetObjectReference.Create(this);
        _isLoading = true;
        _source = await ArticleService.GetViewTopAsync();
        _banner = await CommunityService.GetBannersAsync();
        _isLoading = false;
        await base.OnInitializedAsync();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitWindowWidthListener();
        }
    }
    
    private async Task InitWindowWidthListener()
    {
        await JsRuntime.InvokeVoidAsync("addWindowWidthListener", _objectReference);
    }

    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("removeWindowWidthListener", _objectReference);
        _objectReference?.Dispose();
    }
    
    [JSInvokable]
    public void UpdateWindowWidth(int windowWidth)
    {
        _width = windowWidth;
        StateHasChanged();
    }

    public string BannerStyle()
    {
        if (_width <= 960)
            return $"height:{_width / 1.75}px;margin-bottom: 1em";
        return $"height:{(_width - 240) / 1.75}px;margin-bottom: 1em";
    }
}