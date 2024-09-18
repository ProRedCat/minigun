using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.Home.Controllers;

namespace Minigun.Areas.CrashReporting.Controllers;

[Area("CrashReporting")]
public class CrashReportingController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public CrashReportingController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/crashreporting")]
    public IActionResult Index()
    {
        return View();
    }
}