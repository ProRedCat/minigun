using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Minigun.Areas.Home.Controllers;
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
    
        return RedirectToAction("FullCrashPage", new { applicationIdentifier = firstApplication.identifier });
    }
    
    [HttpGet("/crashreporting/{applicationIdentifier}")]
    public async Task<IActionResult> FullCrashPage(string applicationIdentifier)
    {
        var applications = await _raygunApiService.ListApplicationsAsync(100);

        if (applications == null || !applications.Any())
        {
            return NotFound("No applications found.");
        }

        var selectedApplication = applications.FirstOrDefault(app => app.identifier == applicationIdentifier);
        if (selectedApplication != null)
        {
            // TODO: Make this nicer
            applications.Remove(selectedApplication);
            applications.Insert(0, selectedApplication);
        }
        
        Console.WriteLine(JsonSerializer.Serialize(applications));
        
        // TODO: Not completely sold on this still
        ViewData["Applications"] = applications;

        return View("Index");
    }
}