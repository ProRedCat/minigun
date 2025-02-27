using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.Home.Controllers;
using Minigun.Areas.Rum.Models;
using Minigun.Models;
using Minigun.Services;

namespace Minigun.Areas.Rum.Controllers;

[Area("Rum")]
public class RumController : Controller
{
    private readonly ILogger<RumController> _logger;
    private readonly IRaygunApiService _raygunApiService;

    public RumController(ILogger<RumController> logger, IRaygunApiService raygunApiService)
    {
        _logger = logger;
        _raygunApiService = raygunApiService;
    }
    
    [HttpGet("/rum")]
    public async Task<IActionResult> Index([FromQuery] string? applicationIdentifier)
    {
        if (applicationIdentifier == null)
        {
            var applications = await _raygunApiService.ListApplicationsAsync(100);

            if (applications.Count == 0)
            {
                return NotFound("No applications found.");
            }

            var firstApplication = applications.First();

            return RedirectToAction("FullRumPage", new { applicationIdentifier = firstApplication.Identifier });
        }
        
        Response.Headers.Append("HX-Push", $"/rum/{applicationIdentifier}/");
        
        return PartialView("Index", new RumViewModel(new List<HistogramData>()));
    }

    [HttpGet("/rum/{applicationIdentifier}")]
    public IActionResult FullRumPage(string applicationIdentifier)
    {
        return View("Index", new RumViewModel(new List<HistogramData>()));
    }

    [HttpGet("/rum/load-time-histogram")]
    public async Task<IActionResult> LoadTimeHistogramPartial(
        [FromQuery] string applicationIdentifier,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime
    )
    {
        var histogram = await _raygunApiService.GetRumHistogramAsync(
            applicationId: applicationIdentifier,
            start: startTime,
            end: endTime,
            metrics: ["loadTime"]
        );

        return PartialView("_LoadTimeHistogram", histogram);
    }

    public IActionResult CoreWebVitalsPartial()
    {
        return Content($"I am Core Web Vital partial {DateTime.Now}");
    }
}