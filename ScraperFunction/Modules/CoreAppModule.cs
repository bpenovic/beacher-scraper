using System;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ScraperLib;
using ScraperLib.DAL;
using ScraperLib.DomainServices;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Modules
{
    /// <inheritdoc />
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
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ScraperDbContext>(options =>
            {
                options.UseSqlServer(connectionString, x =>
                {
                    x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    x.UseNetTopologySuite();
                });
            }, ServiceLifetime.Transient);
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IMarkerService, MarkerService>();
            services.AddSingleton<IQualityService, QualityService>();
            services.AddSingleton<IDetailsService, DetailsService>();
            services.Configure<AppSettings>(configuration);

            services.AddHealthChecks().AddDbContextCheck<ScraperDbContext>();
        }
    }
}
