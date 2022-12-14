using Dpz.Client.Data;
using Dpz.Client.Models;
using Microsoft.AspNetCore.Components;

namespace Dpz.Client.Pages;

public partial class Index
{
    private bool _isLoading = false;

    private List<ArticleMiniModel> _source = new();
    
    [Inject]
    private ArticleService ArticleService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        _source = await ArticleService.GetViewTopAsync();
        _isLoading = false;
        await base.OnInitializedAsync();
    }
}