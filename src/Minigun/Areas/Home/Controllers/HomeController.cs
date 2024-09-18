using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.Home.Controllers;

[Area("home")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/register-pat")]
    public async Task<IActionResult> RegisterPat([FromForm] string patToken)
    {
        if (!await IsValidPatToken(patToken))
            return Content("<p class='text-red-600'>Invalid PAT token. Please try again.</p>");
        
        Response.Cookies.Append("RaygunPAT", patToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(30)
        });
        
        Response.Headers.Append("HX-Redirect", Url.Action("Index", "CrashReporting",  new { area = "CrashReporting" }));
        
        return new EmptyResult();
    }

    private async Task<bool> IsValidPatToken(string patToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {patToken}");

        try
        {
            var response = await client.GetAsync("https://api.raygun.com/v3/applications?count=100");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
