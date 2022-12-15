using System.Net.Http.Json;
using System.Text.Json;
using Dpz.Client.Library;
using Dpz.Client.Models;

namespace Dpz.Client.Data;

public class MumbleService
{
    private readonly HttpClient _httpClient;

    public MumbleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IPagedList<MumbleModel>> GetPageAsync(int pageIndex, int pageSize, string content)
    {
        var parameter = new Dictionary<string, string>
        {
            { nameof(pageIndex), pageIndex.ToString() },
            { nameof(pageSize), pageSize.ToString() },
            { nameof(content), content },
        };
        var result = await _httpClient.ToPagedListAsync<MumbleModel>("/api/Mumble", parameter);
        return result;
    }

    public async Task<MumbleModel> GetMumbleAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<MumbleModel>($"/api/Mumble/{id}", new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<MumbleModel> LikeAsync(string id)
    {
        var response = await _httpClient.PostAsync($"/api/Mumble/like/{id}", new StringContent(""));
        return await response.Content.ReadFromJsonAsync<MumbleModel>();
    }
}