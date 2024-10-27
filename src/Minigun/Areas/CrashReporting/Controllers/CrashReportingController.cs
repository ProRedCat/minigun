using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.CrashReporting.Models;
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
    public async Task<IActionResult> FullCrashPage(string applicationIdentifier)
    {
        var endTime = DateTime.Now;
        var startTime = endTime.AddDays(-7);

        var errorGroupsTask = _raygunApiService.ListErrorGroupsAsync(applicationIdentifier, orderby: ["lastOccurredAt desc"]);
        var timeseriesTask = _raygunApiService.GetErrorTimeseriesAsync(applicationIdentifier, startTime, endTime);

        var errorGroups = await errorGroupsTask;
        var timeseries = await timeseriesTask;
        
        var filteredGroups = errorGroups
            .Where(e => e.LastOccurredAt > startTime)
            .ToList();

        var viewModel = new CrashReportingViewModel(
            filteredGroups,
            timeseries
        );

        return View("Index", viewModel);
    }

    [HttpGet("/crashreporting/{applicationIdentifier}/error-groups")]
    public async Task<IActionResult> ErrorGroupsPartial(string applicationIdentifier)
    {
        var endTime = DateTime.Now;
        var startTime = endTime.AddDays(-7);

        var errorGroupsTask =
            _raygunApiService.ListErrorGroupsAsync(applicationIdentifier, orderby: ["lastOccurredAt desc"]);
        var timeseriesTask = _raygunApiService.GetErrorTimeseriesAsync(applicationIdentifier, startTime, endTime);
        
        var errorGroups = await errorGroupsTask;
        var timeseries = await timeseriesTask;
        
        var filteredGroups = errorGroups
            .Where(e => e.LastOccurredAt > startTime)
            .ToList();

        var viewModel = new CrashReportingViewModel(
            filteredGroups,
            timeseries
        );

        return PartialView("_ErrorSummary", viewModel);
    }
}