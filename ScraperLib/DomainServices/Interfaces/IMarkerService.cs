using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IMarkerService
    {
        Task<List<Quality>> GetQualityAsync(string url, Marker marker);
        Task<IEnumerable<Marker>> GetMarkersAsync(string url);
        Task<Profile> GetDetailsAsync(string url, Marker marker);
    }
}
