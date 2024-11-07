using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.Deployments.Controllers;

[Area("Deployments")]
public class DeploymentsController : Controller
{
    private readonly ILogger<DeploymentsController> _logger;

    public DeploymentsController(ILogger<DeploymentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/deployments")]
    public IActionResult Index()
    {
        return PartialView();
    }
}