using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace ScraperLib.DAL
{
    public class ScraperDbFactory : IDesignTimeDbContextFactory<ScraperDbContext>
    {
        private readonly AppSettings _settings;

        public ScraperDbFactory()
        {

        }
        public ScraperDbFactory(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }
        public ScraperDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ScraperDbContext>();

            var connectionString = _settings.ConnectionStrings.DefaultConnection;
            builder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());

            return new ScraperDbContext(builder.Options);
        }
        public static DbContextOptionsBuilder<ScraperDbContext> GetOptionsBuilder(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScraperDbContext>();
            optionsBuilder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
            optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) }));
            return optionsBuilder;
        }
    }
}
