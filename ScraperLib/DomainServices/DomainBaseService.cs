using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ScraperLib.DAL;

namespace ScraperLib.DomainServices
{
    public abstract class DomainBaseService
    {
        private readonly AppSettings _settings;
        protected DomainBaseService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        protected ScraperDbContext GetDbContext(bool tracking)
        {
            var scraperDbContext = new ScraperDbContext(_settings.ConnectionStrings.DefaultConnection);
            scraperDbContext.ChangeTracker.QueryTrackingBehavior = tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return scraperDbContext;
        }
    }
}