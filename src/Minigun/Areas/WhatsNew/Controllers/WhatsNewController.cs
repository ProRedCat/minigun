using Microsoft.AspNetCore.Mvc;

namespace Minigun.Areas.WhatsNew.Controllers;

[Area("WhatsNew")]
public class WhatsNewController : Controller
{
    private readonly ILogger<WhatsNewController> _logger;

    public WhatsNewController(ILogger<WhatsNewController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/whatsnew")]
    public IActionResult Index()
    {
        return PartialView();
    }
}