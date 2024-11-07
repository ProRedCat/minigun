namespace Minigun.Models;

public class ApplicationsPartialModel
{
    public required IEnumerable<Application> Applications { get; set; }
    public string? SelectedApplicationId { get; set; }
}
