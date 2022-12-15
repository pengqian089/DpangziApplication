using Dpz.Client.Data;
using Dpz.Client.Library;
using Dpz.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dpz.Client.Pages;

public partial class MumbleList
{
    private bool _isLoading = false;

    private const int PageSize = 10;

    private IPagedList<MumbleModel> _source = new PagedList<MumbleModel>(new List<MumbleModel>(), 1, 10);
    
    [Parameter]
    public int PageIndex { get; set; }

    private MudPagination _mudPagination;
    
    [Inject]
    private MumbleService MumbleService { get; set; }

    [Inject]
    private NavigationManager Navigation { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        PageIndex = PageIndex == 0 ? 1 : PageIndex;
        await base.OnInitializedAsync();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        _source = await MumbleService.GetPageAsync(PageIndex, PageSize, "");
        _isLoading = false;
        PageIndex = _source.CurrentPageIndex;
        await base.OnParametersSetAsync();
    }
    
    private void ToPageAsync(int page)
    {
        PageIndex = page;
        Navigation.NavigateTo($"/mumble/{page}");
    }

    private async Task LikeAsync(string id)
    {
        var mumble = await MumbleService.LikeAsync(id);
        var likeMumble = _source.FirstOrDefault(x => x.Id == id);
        if (likeMumble != null)
            likeMumble.Like = mumble.Like;
        StateHasChanged();
    }
}