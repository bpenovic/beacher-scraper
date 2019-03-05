using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IDetailsService
    {
        Task<Profile> ScrapeDetailsAsync(string url, Marker marker);
    }
}
