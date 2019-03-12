using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ScraperLib.DAL;

namespace ScraperLib.DomainServices
{
    public abstract class DomainBaseService
    {
        private readonly AppSettings _settings;
        protected static HttpClient HttpClient { set; get; }
        protected DomainBaseService(IOptions<AppSettings> settings, HttpClient client)
        {
            _settings = settings.Value;
            HttpClient = client;
        }

        protected ScraperDbContext GetDbContext(bool tracking)
        {
            var scraperDbContext = new ScraperDbContext(_settings.ConnectionStrings.DefaultConnection);
            scraperDbContext.ChangeTracker.QueryTrackingBehavior = tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return scraperDbContext;
        }
    }
}