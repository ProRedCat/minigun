using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.Users.Controllers;

[Area("Users")]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/users")]
    public IActionResult Index()
    {
        return View();
    }
}