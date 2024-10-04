using Microsoft.AspNetCore.Mvc;
using Minigun.Models;
using Minigun.Services;

namespace Minigun.ViewComponents;

public class ApplicationDropdownViewComponent : ViewComponent
{
    private readonly IRaygunApiService _raygunApiService;

    public ApplicationDropdownViewComponent(IRaygunApiService raygunApiService)
    {
        _raygunApiService = raygunApiService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var route = Request.Path.ToString().Split("/")[1];
        var applications = await _raygunApiService.ListApplicationsAsync(100) ?? [];

        var model = new ApplicationsPartialModel
        {
            Applications = applications,
            HtmxContext = GetHtmxContext(route)
        };
        
        
        //TODO: Selected application should be first in view
        return View("/Areas/Shared/_ApplicationsPartial.cshtml", model);
    }

    private static ApplicationsPartialHtmxModel GetHtmxContext(string route)
    {
        return route switch
        {
            "crashreporting" => new ApplicationsPartialHtmxModel
            {
                Controller = "CrashReporting",
                Action = "ErrorGroupsPartial",
                RoutePrefix = route
            },
            "rum" => new ApplicationsPartialHtmxModel
            {
                Controller = "Rum",
                Action = "CoreWebVitalsPartial",
                RoutePrefix = route
            },
            _ => throw new ArgumentException($"Unknown route: {route}")
        };
    }
}