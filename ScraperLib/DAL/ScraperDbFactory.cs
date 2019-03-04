using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

//using Microsoft.Extensions.Configuration;

namespace ScraperLib.DAL
{
    public class ScraperDbFactory : IDesignTimeDbContextFactory<ScraperDbContext>
    {
        public ScraperDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ScraperDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
            return new ScraperDbContext(builder.Options);
        }
    }
}
