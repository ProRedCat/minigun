using Microsoft.AspNetCore.Mvc;
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

        if (applications == null || !applications.Any())
        {
            return NotFound("No applications found.");
        }

        var firstApplication = applications.First();

        return RedirectToAction("FullCrashPage", new { applicationIdentifier = firstApplication.Identifier });
    }

    [HttpGet("/crashreporting/{applicationIdentifier}")]
    public async Task<IActionResult> FullCrashPage(string applicationIdentifier)
    {
        // TODO: See if there is a nicer way to handle this, as this method is identical to the one below
        // this may be handled once we move to OnLoad fetching of partials rather than full page load
        var errorGroups = await _raygunApiService.ListErrorGroupsAsync(applicationIdentifier) ?? [];
        
        return View("Index", errorGroups);
    }
    
    [HttpGet("/crashreporting/{applicationIdentifier}/error-groups")]
    public async Task<IActionResult> ErrorGroupsPartial(string applicationIdentifier)
    {
        var errorGroups = await _raygunApiService.ListErrorGroupsAsync(applicationIdentifier) ?? [];
        
        return PartialView("_ErrorGroups", errorGroups);
    }
}