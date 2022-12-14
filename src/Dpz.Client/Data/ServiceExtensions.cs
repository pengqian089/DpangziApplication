using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Dpz.Client.Library;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dpz.Client.Data;

public static class ServiceExtensions
{
    private static readonly Dictionary<string, string> RequestEtag = new();

    private static readonly Dictionary<string, object> ClientCache = new();

    private static string HandleParameter(HttpClient client, string url, Dictionary<string, string> parameters)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (parameters != null && parameters.Any())
        {
            var index = url.IndexOf("?", StringComparison.CurrentCultureIgnoreCase);
            var query = index >= 0 ? url.Substring(index) : "";
            var queryString = HttpUtility.ParseQueryString(query);
            foreach (var item in parameters)
            {
                queryString.Add(item.Key, item.Value);
            }

            if (index >= 0)
            {
                url = url.Substring(0, index + 1) + queryString;
            }
            else
            {
                url += "?" + queryString;
            }
        }

        return url;
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(
        this HttpClient client,
        string url,
        Dictionary<string, string> parameters = null,
        JsonConverter converter = null)
    {
        var requestUrl = HandleParameter(client, url, parameters);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        var currentUri = request.RequestUri?.ToString() ?? "";
        if (RequestEtag.ContainsKey(currentUri))
        {
            request.Headers.Add("If-None-Match", RequestEtag[currentUri]);
        }

        var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.NotModified && ClientCache.ContainsKey(currentUri))
        {
            return ClientCache[currentUri] as PagedList<T> ?? new PagedList<T>(new List<T>(), 0, 0);
        }
        else
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (converter != null)
                serializerOptions.Converters.Add(converter);

            //var result = await response.Content.ReadAsStringAsync();
            //var list = JsonSerializer.Deserialize<List<T>>(result, serializerOptions);

            var list = await response.Content.ReadFromJsonAsync<List<T>>(serializerOptions);
            var pagination =
                JsonSerializer.Deserialize<Pagination>(response.Headers.GetValues("X-Pagination").First(),
                    serializerOptions) ?? new Pagination();
            if (RequestEtag.ContainsKey(currentUri))
            {
                RequestEtag[currentUri] = response.Headers.ETag?.ToString();
            }
            else
            {
                RequestEtag.Add(currentUri, response.Headers.ETag?.ToString());
            }

            var pagedList = new PagedList<T>(list, pagination.CurrentPage, pagination.PageSize,
                pagination.TotalCount);
            ClientCache[currentUri] = pagedList;
            return pagedList;
        }
    }


    public static T GetQueryString<T>(this NavigationManager navManager, string key)
    {
        var uri = navManager.ToAbsoluteUri(navManager.Uri);

        var valueFromQueryString = HttpUtility.ParseQueryString(uri.Query).Get(key);
        if (valueFromQueryString != null)
        {
            if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
            {
                return (T)(object)valueAsInt;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)valueFromQueryString;
            }

            if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
            {
                return (T)(object)valueAsDecimal;
            }
        }

        return default(T);
    }
}