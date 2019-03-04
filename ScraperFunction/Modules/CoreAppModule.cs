using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScraperLib.DomainServices;
using ScraperLib.DomainServices.Interfaces;

//using ScraperLib.DomainServices;

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
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IMarkerService, MarkerService>();
        }
    }
}
