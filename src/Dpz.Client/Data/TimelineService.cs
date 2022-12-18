using System.Net.Http.Json;
using Dpz.Client.Models;

namespace Dpz.Client.Data;

public class TimelineService
{
    private readonly HttpClient _httpClient;

    public TimelineService(
        HttpClient httpClient
    )
    {
        _httpClient = httpClient;
    }

    public async Task<List<TimelineModel>> GetTimelineAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<TimelineModel>>("/api/Timeline");
        return result;
    }
}