using Dpz.Client.Data;
using Dpz.Client.Models;
using Microsoft.AspNetCore.Components;

namespace Dpz.Client.Pages;

public partial class ArticleDetail
{
    [Parameter]
    public string Id { get; set; }
    
    [Inject] private ArticleService ArticleService { get; set; }
    
    private ArticleModel _article = new();
    
    private bool _loading = false;
    
    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        _article = await ArticleService.GetArticleAsync(Id);
        _loading = false;
        await base.OnParametersSetAsync();
    }
}