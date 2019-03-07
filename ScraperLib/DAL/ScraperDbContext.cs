using Microsoft.EntityFrameworkCore;
using ScraperLib.Models;

namespace ScraperLib.DAL
{
    public class ScraperDbContext : DbContext
    {
        public ScraperDbContext(DbContextOptions<ScraperDbContext> options) : base(options) { }

        public ScraperDbContext(string connectionString) : base(ScraperDbFactory.GetOptionsBuilder(connectionString)
            .Options)
        {
        }
        public DbSet<Marker> Markers { get; set; }
        public DbSet<Quality> Qualities { get; set; }
        public DbSet<Details> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
