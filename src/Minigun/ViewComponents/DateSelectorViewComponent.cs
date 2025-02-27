using Microsoft.AspNetCore.Mvc;
using Minigun.Models;

namespace Minigun.ViewComponents;

public class DateSelectorViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var now = DateTime.Now;
        var minDate = now.AddMonths(-6);
        var defaultStartDate = now.AddDays(-7);

        var model = new DateSelector
        {
            MinDate = minDate.ToString("yyyy-MM-ddTHH:mm"),
            MaxDate = now.ToString("yyyy-MM-ddTHH:mm"),
            SelectedRange = $"{defaultStartDate:MMM d, yyyy h:mm tt} - {now:MMM d, yyyy h:mm tt}",
            StartDate = defaultStartDate,
            EndDate = now
        };

        return View("/Areas/Shared/_DateSelector.cshtml", model);
    }
}