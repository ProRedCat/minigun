using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.Home.Controllers;
using Minigun.Services;

namespace Minigun.Areas.CrashReporting.Controllers;

[Area("CrashReporting")]
public class CrashReportingController : Controller
{
    private readonly ILogger<CrashReportingController> _logger;
    private readonly IRaygunApiService _raygunApiService;

    public CrashReportingController(ILogger<CrashReportingController> logger, IRaygunApiService raygunApiService)
    {
        _logger = logger;
        _raygunApiService = raygunApiService;
    }

    [HttpGet("/crashreporting")]
    public async Task<IActionResult> Index()
    {
        ViewData["Applications"] = await _raygunApiService.ListApplicationsAsync(100);
        Console.WriteLine(JsonSerializer.Serialize(ViewData["Applications"]));
        return View();
    }
}