using Microsoft.AspNetCore.Mvc;
using Minigun.Models;
using Minigun.Services;
using System.Text.Json;

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

        var applications = await GetAllApplicationsAsync();
        
        // Sort applications alphabetically
        applications = applications.OrderBy(a => a.Name).ToList();
        
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

    private async Task<List<Application>> GetAllApplicationsAsync()
    {
        var allApplications = new List<Application>();
        var offset = 0;
        const int pageSize = 100;
        bool hasMoreData;

        do
        {
            var batch = await _raygunApiService.ListApplicationsAsync(pageSize, offset);
            allApplications.AddRange(batch);
            
            hasMoreData = batch.Count == pageSize;
            offset += pageSize;
        } 
        while (hasMoreData);

        return allApplications;
    }
}