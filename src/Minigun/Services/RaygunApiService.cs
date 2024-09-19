using System.Net.Http.Headers;
using System.Text.Json;
using Minigun.Models;

namespace Minigun.Services;

public class RaygunApiService : IRaygunApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

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
    
    public async Task<List<Application>?> ListApplicationsAsync(int? count = null, int? offset = null, string[]? orderby = null)
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
        return JsonSerializer.Deserialize<List<Application>>(responseContent);
    }
}