using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IQualityService
    {
        Task ScrapeQualityAsync(string url, IEnumerable<Marker> markers);
        Task<List<Quality>> ScrapeQualityAsync(string url, Marker marker);
        Task<IEnumerable<Quality>> GetQualitiesForMarkerAsync(int markerId);
    }
}
