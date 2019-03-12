using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions.HttpTrigger
{
    public static class HttpGetQuality
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("HttpGetQuality")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("GetQuality function processed a request.");

            List<Quality> quality;
            var qualityService = Container.GetRequiredService<IQualityService>();
            var markerService = Container.GetRequiredService<IMarkerService>();

            var url = $"{Endpoints.Quality}?{Parameters.Year}=2018&{Parameters.Cycle}=-2&{Parameters.Language}=eng&{Parameters.View}=,&{Parameters.CycleView}=,ci,";

            if (Int32.TryParse(req.Query["markerId"], out var markerId))
            {
                var marker = await markerService.GetMarkerByIdAsync(markerId);
                quality = await qualityService.ScrapeAndSaveQualityAsync(url, marker);
            }
            else
            {
                int? skip = null;
                int? take = null;
                if (Int32.TryParse(req.Query["skip"], out int skipParam))
                {
                    skip = skipParam;
                    if (Int32.TryParse(req.Query["take"], out int takeParam))
                        take = takeParam;
                }

                var markers = await markerService.GetMarkersAsync(skip, take);
                quality = await qualityService.ScrapeAndSaveQualityAsync(url, markers);
            }

            return new OkObjectResult($"{JsonConvert.SerializeObject(quality)}");
        }
    }
}
