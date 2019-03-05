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
using Microsoft.Extensions.Options;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions
{
    public static class GetQuality
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("GetQuality")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetQuality function processed a request.");

            var quality = new List<Quality>();
            var markerService = Container.GetRequiredService<IMarkerService>();
            var endPoints = Container.GetRequiredService<IOptions<AppSettings>>().Value.DataEndpoints;
            var url = $"{endPoints.MarkerQuality}?{Parameters.Year}=2018&{Parameters.Cycle}=-2&{Parameters.Language}=eng&{Parameters.View}=,&{Parameters.CycleView}=,ci,";

            if (Int32.TryParse(req.Query["markerId"], out var markerId))
            {
                var marker = await markerService.GetMarkerById(markerId);
                quality = await markerService.ScrapeQualityAsync(url, marker);
            }
            else
            {
                var markers = await markerService.GetMarkersAsync();
                await markerService.ScrapeQualityAsync(url, markers);
            }

            return new OkObjectResult($"GetQuality function works! \n{JsonConvert.SerializeObject(quality)}");
        }
    }
}
