using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IMarkerService
    {
        Task ScrapeQualityAsync(string url, IEnumerable<Marker> markers);
        Task<List<Quality>> ScrapeQualityAsync(string url, Marker marker);
        Task<IEnumerable<Marker>> ScrapeMarkersAsync(string url);
        Task<Profile> ScrapeDetailsAsync(string url, Marker marker);
        Task<IEnumerable<Marker>> GetMarkersAsync();
    }
}
