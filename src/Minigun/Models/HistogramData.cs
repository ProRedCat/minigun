namespace Minigun.Models;

public class HistogramData
{
    public string? Metric { get; set; }
    public List<HistogramBucket> Buckets { get; set; } = [];
}

public class HistogramBucket
{
    public double Key { get; set; }
    public string Range { get; set; } = string.Empty;
    public int Count { get; set; }
}