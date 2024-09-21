namespace Minigun.Models;

public class Application
{
    public string? Identifier { get; set; }
    public string? PlanIdentifier { get; set; }
    public string? Name { get; set; }
    public string? ApiKey { get; set; }
    public bool HasSentCrashData { get; set; }
    public bool HasSentApmData { get; set; }
    public bool HasSentRumData { get; set; }
    public string? RumDataType { get; set; }
}