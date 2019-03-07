using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IDetailsService
    {
        Task<List<Details>> ScrapeAndSaveDetailsAsync(string url, IEnumerable<Marker> markers);
        Task<Details> ScrapeAndSaveDetailsAsync(string url, Marker marker);
    }
}
