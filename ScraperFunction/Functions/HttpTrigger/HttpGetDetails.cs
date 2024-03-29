using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions.HttpTrigger
{
    public static class HttpGetDetails
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("HttpGetDetails")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetDetails function processed a request.");

            var markerService = Container.GetRequiredService<IMarkerService>();
            var detailsService = Container.GetRequiredService<IDetailsService>();

            var url = $"{Endpoints.Details}?{Parameters.Season}=2018&{Parameters.Language}=eng";

            if (Int32.TryParse(req.Query["markerId"], out int markerId))
            {
                var marker = await markerService.GetMarkerByIdAsync(markerId);
                var markerDetails = await detailsService.ScrapeAndSaveDetailsAsync(url, marker);
                return new OkObjectResult($"{JsonConvert.SerializeObject(markerDetails)}");
            }

            int? skip = null;
            int? take = null;
            if (Int32.TryParse(req.Query["skip"], out int skipParam))
            {
                skip = skipParam;
                if (Int32.TryParse(req.Query["take"], out int takeParam))
                    take = takeParam;
            }

            var markers = await markerService.GetMarkersAsync(skip, take);
            var markersDetails = await detailsService.ScrapeAndSaveDetailsAsync(url, markers);
            return new OkObjectResult($"{JsonConvert.SerializeObject(markersDetails)}");
        }
    }
}

