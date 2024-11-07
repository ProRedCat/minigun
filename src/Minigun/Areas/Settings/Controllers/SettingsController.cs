using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.Settings.Controllers;

[Area("Settings")]
public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ILogger<SettingsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/settings")]
    public IActionResult Index()
    {
        return PartialView();
    }
}