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
            var sarahahV2DbContext = new ScraperDbContext(_settings.ConnectionStrings.DefaultConnection);
            sarahahV2DbContext.ChangeTracker.QueryTrackingBehavior = tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return sarahahV2DbContext;
        }
    }
}