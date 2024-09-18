using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minigun.Models;

namespace Minigun.Areas.Home.Controllers;

[Area("home")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
