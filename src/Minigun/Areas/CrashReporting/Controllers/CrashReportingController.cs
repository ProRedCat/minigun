using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.CrashReporting.Models;
using Minigun.Models;
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
        var applications = await _raygunApiService.ListApplicationsAsync(100);

        if (applications.Count == 0)
        {
            return NotFound("No applications found.");
        }

        var firstApplication = applications.First();

        return RedirectToAction("FullCrashPage", new { applicationIdentifier = firstApplication.Identifier });
    }

    [HttpGet("/crashreporting/{applicationIdentifier}")]
    public IActionResult FullCrashPage(string applicationIdentifier)
    {
        var viewModel = new CrashReportingViewModel(
            new List<ErrorGroup>(),
            new List<TimeseriesData>()
        );

        return View("Index", viewModel);
    }

    [HttpGet("/crashreporting/error-groups")]
    public async Task<IActionResult> ErrorGroupsPartial([FromQuery] string applicationIdentifier)
    {
        var endTime = DateTime.Now;
        var startTime = endTime.AddDays(-7);

        var errorGroups = await _raygunApiService.ListErrorGroupsAsync(applicationIdentifier, orderby: ["lastOccurredAt desc"]);
        
        var filteredGroups = errorGroups
            .Where(e => e.LastOccurredAt > startTime)
            .ToList();
        
        Response.Headers.Append("HX-Push", $"/crashreporting/{applicationIdentifier}/");

        return PartialView("_ErrorGroups", filteredGroups);
    }
    
    [HttpGet("/crashreporting/error-timeseries")]
    public async Task<IActionResult> ErrorTimeseriesPartial([FromQuery] string applicationIdentifier)
    {
        var endTime = DateTime.Now;
        var startTime = endTime.AddDays(-7);
        
        var errorTimeseries = await _raygunApiService.GetErrorTimeseriesAsync(applicationIdentifier, startTime, endTime);
        
        Response.Headers.Append("HX-Push", $"/crashreporting/{applicationIdentifier}/");

        return PartialView("_ErrorTimeseries", errorTimeseries);
    }
}