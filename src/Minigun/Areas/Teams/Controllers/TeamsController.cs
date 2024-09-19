using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.Teams.Controllers;

[Area("Teams")]
public class TeamsController : Controller
{
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ILogger<TeamsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/teams")]
    public IActionResult Index()
    {
        return View();
    }
}