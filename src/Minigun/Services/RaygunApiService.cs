using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Minigun.Models;

namespace Minigun.Services;

public class RaygunApiService : IRaygunApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly JsonSerializerOptions JsonOptions;

    static RaygunApiService()
    {
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public RaygunApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _httpClient.BaseAddress = new Uri("https://api.raygun.com/v3/");
    }

    private void SetAuthorizationHeader()
    {
        var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
        if (cookies != null && cookies.TryGetValue("RaygunPAT", out var token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<Application>> ListApplicationsAsync(
        int? count = null,
        int? offset = null,
        string[]? orderby = null)
    {
        SetAuthorizationHeader();

        var query = new List<string>();
        if (count.HasValue) query.Add($"count={count.Value}");
        if (offset.HasValue) query.Add($"offset={offset.Value}");
        if (orderby != null && orderby.Length > 0) query.Add($"orderby={string.Join(",", orderby)}");

        var url = $"applications?" + string.Join("&", query);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Application>>(responseContent, JsonOptions) ?? [];
    }

    public async Task<List<ErrorGroup>> ListErrorGroupsAsync(
        string applicationId,
        int? count = null,
        int? offset = null,
        string[]? orderby = null)
    {
        SetAuthorizationHeader();

        var query = new List<string>();
        if (count.HasValue) query.Add($"count={count.Value}");
        if (offset.HasValue) query.Add($"offset={offset.Value}");
        if (orderby != null && orderby.Length > 0) query.Add($"orderby={string.Join(",", orderby)}");

        var url = $"applications/{applicationId}/error-groups?" + string.Join("&", query);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ErrorGroup>>(responseContent, JsonOptions) ?? [];
    }

    public async Task<List<TimeseriesData>> GetErrorTimeseriesAsync(
        string applicationId,
        DateTime start,
        DateTime end,
        List<string>? errorGroupIds = null)
    {
        SetAuthorizationHeader();

        var requestBody = new
        {
            start = start.ToUniversalTime(),
            end = end.ToUniversalTime(),
            granularity = "1h",
            aggregation = "count",
            metrics = new[] { "errorInstances" },
            filter = errorGroupIds?.Any() == true
                ? $"errorGroupIdentifier IN ({string.Join(", ", errorGroupIds.Select(id => $"'{id}'"))})"
                : null
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"metrics/{applicationId}/errors/time-series", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TimeseriesData>>(responseContent, JsonOptions) ?? [];
    }
    
    public async Task<List<HistogramData>> GetRumHistogramAsync(
        string applicationId,
        DateTime start,
        DateTime end,
        string[] metrics,
        string? filter = null)
    {
        SetAuthorizationHeader();

        var requestBody = new
        {
            start = start.ToUniversalTime(),
            end = end.ToUniversalTime(),
            metrics = metrics,
            filter = filter
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"metrics/{applicationId}/pages/histogram", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<HistogramData>>(responseContent, JsonOptions) ?? [];
    }
}