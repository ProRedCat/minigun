namespace Minigun.Models;

public class DateSelector
{
    public string? MinDate { get; set; }
    public string? MaxDate { get; set; }
    public string? SelectedRange { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}