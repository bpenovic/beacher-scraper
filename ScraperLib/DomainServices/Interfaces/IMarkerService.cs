using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IMarkerService
    {
        Task<IEnumerable<Marker>> ScrapeMarkersAsync(string url);
        Task<IEnumerable<Marker>> GetMarkersAsync();
        Task<Marker> GetMarkerByIdAsync(int id);
    }
}
