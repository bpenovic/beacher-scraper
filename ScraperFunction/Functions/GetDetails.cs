using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions
{
    public static class GetDetails
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("GetDetails")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetDetails function processed a request.");

            var markerDetails = new Profile();
            var markerService = Container.GetRequiredService<IMarkerService>();
            var detailsService = Container.GetRequiredService<IDetailsService>();

            var url = $"{Endpoints.Details}?{Parameters.Season}=2018&{Parameters.Language}=eng";

            if (Int32.TryParse(req.Query["markerId"], out int markerId))
            {
                var marker = await markerService.GetMarkerByIdAsync(markerId);
                markerDetails = await detailsService.ScrapeDetailsAsync(url, marker);
            }

            return new OkObjectResult($"{JsonConvert.SerializeObject(markerDetails)}");
        }
    }
}

