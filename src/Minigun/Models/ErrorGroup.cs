namespace Minigun.Models;

public class ErrorGroup
{
    public string? Identifier { get; set; }
    public string? ApplicationIdentifier { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
    public DateTime LastOccurredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ErrorGroupResolvedIn? ResolvedIn { get; set; }
    public bool DiscardNewOccurrences { get; set; }
    public string? ApplicationUrl { get; set; }
}

public class ErrorGroupResolvedIn
{
    public string? Version { get; set; }
    public bool DiscardFromPreviousVersions { get; set; }
}