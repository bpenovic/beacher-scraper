using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace ScraperLib.DAL
{
    public class ScraperDbFactory : IDesignTimeDbContextFactory<ScraperDbContext>
    {
        private readonly AppSettings _settings;
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
    }
}
