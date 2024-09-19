using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.Home.Controllers;

namespace Minigun.Areas.Rum.Controllers;

[Area("Rum")]
public class RumController : Controller
{
    private readonly ILogger<RumController> _logger;

    public RumController(ILogger<RumController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/rum")]
    public IActionResult Index()
    {
        return View();
    }
}