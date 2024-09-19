using Microsoft.AspNetCore.Mvc;
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
        var applications = await _raygunApiService.ListApplicationsAsync(100) ?? [];

        //TODO: Selected application should be first in view
        return View("/Areas/Shared/_ApplicationsPartial.cshtml", applications);
    }
}