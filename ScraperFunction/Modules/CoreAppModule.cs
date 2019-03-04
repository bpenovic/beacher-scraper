using System;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScraperFunction;
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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ScraperDbContext>(options =>
                options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IMarkerService, MarkerService>();
            services.Configure<AppSettings>(configuration.GetSection("ConnectionStrings"));
            services.AddHealthChecks().AddDbContextCheck<ScraperDbContext>();
        }
    }
}
