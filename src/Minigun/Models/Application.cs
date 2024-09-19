namespace Minigun.Models;

public class Application
{
    public string? identifier { get; set; }
    public string? planIdentifier { get; set; }
    public string? name { get; set; }
    public string? apiKey { get; set; }
    public bool hasSentCrashData { get; set; }
    public bool hasSentApmData { get; set; }
    public bool hasSentRumData { get; set; }
    public string? rumDataType { get; set; }
}