using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IQualityService
    {
        Task<List<Quality>> ScrapeAndSaveQualityAsync(string url, IEnumerable<Marker> markers);
        Task<List<Quality>> ScrapeAndSaveQualityAsync(string url, Marker marker);
        Task<IEnumerable<Quality>> GetQualitiesForMarkerAsync(int markerId);
    }
}
