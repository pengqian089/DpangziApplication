using System.Net.Http.Json;
using Dpz.Client.Models;

namespace Dpz.Client.Data;

public class CommunityService
{
    private readonly HttpClient _httpClient;

    public CommunityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
        
    public async Task<List<PictureModel>> GetBannersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<PictureModel>>("/api/Community/getBanners");
    }
}