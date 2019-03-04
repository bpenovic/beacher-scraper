using System;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScraperLib.DAL;
using ScraperLib.DomainServices;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Modules
{
    /// <summary>
    /// This represents the module entity for dependencies.
    /// </summary>
    public class CoreAppModule : Module
    {
        /// <inheritdoc />
        public override void Load(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<ScraperDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IMarkerService, MarkerService>();

            services.AddHealthChecks().AddDbContextCheck<ScraperDbContext>();
        }
    }
}
