using Minigun.Models;

namespace Minigun.Areas.Rum.Models;

public class RumViewModel
{
    public RumViewModel(IEnumerable<HistogramData> loadTimeHistogram)
    {
        LoadTimeHistogram = loadTimeHistogram;
    }

    public IEnumerable<HistogramData> LoadTimeHistogram { get; set; }
}