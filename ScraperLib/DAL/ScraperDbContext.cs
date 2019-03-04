using Microsoft.EntityFrameworkCore;

namespace ScraperLib.DAL
{
    public class ScraperDbContext : DbContext
    {
        public ScraperDbContext(DbContextOptions<ScraperDbContext> options)
            : base(options)
        { }
    }
}
