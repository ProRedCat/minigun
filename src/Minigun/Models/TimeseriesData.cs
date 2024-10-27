namespace Minigun.Models;

public class TimeseriesPoint
{
    public DateTime Time { get; set; }
    public double Value { get; set; }
}

public class TimeseriesData
{
    public string? Aggregation { get; set; }
    public string? Metric { get; set; }
    public List<TimeseriesPoint>? Series { get; set; }
}