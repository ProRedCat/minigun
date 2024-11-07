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
        var pathParts = Request.Path.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);
        var selectedId = pathParts.Length > 1 ? pathParts[1] : null;

        var applications = await _raygunApiService.ListApplicationsAsync(100);
        
        // Find selected application and move it to first position if found
        var selectedApp = applications.FirstOrDefault(a => a.Identifier == selectedId);
        if (selectedApp != null && applications.Count > 0)
        {
            applications = applications
                .Where(a => a.Identifier != selectedId)
                .Prepend(selectedApp)
                .ToList();
        }

        var model = new ApplicationsPartialModel
        {
            Applications = applications,
            SelectedApplicationId = selectedId
        };
        
        return View("/Areas/Shared/_ApplicationsPartial.cshtml", model);
    }
}