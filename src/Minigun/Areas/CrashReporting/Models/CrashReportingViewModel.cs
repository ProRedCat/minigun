using Minigun.Models;

namespace Minigun.Areas.CrashReporting.Models;

public class CrashReportingViewModel
{
    public CrashReportingViewModel(IEnumerable<ErrorGroup> errorGroups, IEnumerable<TimeseriesData> errorsTimeseries)
    {
        ErrorGroups = errorGroups;
        ErrorsTimeseries = errorsTimeseries;
    }

    public IEnumerable<ErrorGroup> ErrorGroups { get; set; }
    public IEnumerable<TimeseriesData> ErrorsTimeseries { get; set; }
}