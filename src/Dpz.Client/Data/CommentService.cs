using System.Net.Http.Json;
using System.Text.Json;
using Dpz.Client.Enum;
using Dpz.Client.Library;
using Dpz.Client.Models;

namespace Dpz.Client.Data;

public class CommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IPagedList<CommentModel>> GetPageAsync(CommentNode node, string relation, int pageIndex = 1,
        int pageSize = 10)
    {
        var parameter = new Dictionary<string, string>
        {
            { nameof(pageIndex), pageIndex.ToString() },
            { nameof(pageSize), pageSize.ToString() },
            { nameof(node), node.ToString() },
            { nameof(relation), relation },
        };
        return await _httpClient.ToPagedListAsync<CommentModel>("/api/Comment/page", parameter);
    }

    public async Task<IPagedList<CommentModel>> SendAsync(SendComment comment, int pageSize = 5)
    {
        var response = await _httpClient.PostAsync($"/api/Comment?pageSize={pageSize}", JsonContent.Create(comment));

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await response.Content.ReadFromJsonAsync<List<CommentModel>>(serializerOptions);

        response.Headers.TryGetValues("X-Pagination", out var pageInformation);
        var pagination =
            JsonSerializer.Deserialize<Pagination>(pageInformation?.FirstOrDefault() ?? "{}",
                serializerOptions) ?? new Pagination();

        return new PagedList<CommentModel>(result, pagination.CurrentPage, pagination.PageSize, pagination.TotalCount);
    }
}