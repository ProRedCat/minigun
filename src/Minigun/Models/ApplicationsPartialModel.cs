namespace Minigun.Models;

public class ApplicationsPartialModel
{
    public required IEnumerable<Application> Applications { get; set; }
    public required ApplicationsPartialHtmxModel HtmxContext { get; set; }
}

public class ApplicationsPartialHtmxModel
{
    public required string Controller { get; set; }
    public required string Action { get; set; }
    public required string RoutePrefix { get; set; }
    public required string LoadingIndicator { get; set; }
}