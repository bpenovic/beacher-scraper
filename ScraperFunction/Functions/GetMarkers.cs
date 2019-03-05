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
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions
{

    public static class GetMarkers
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("GetMarkers")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetMarkers function processed a request.");

            var markerService = Container.GetRequiredService<IMarkerService>();
            var endPoints = Container.GetRequiredService<IOptions<AppSettings>>().Value.DataEndpoints;
            var url =$"{endPoints.Markers}?{Parameters.Year}=2018&{Parameters.Filter}=&{Parameters.Cycle}=-2&{Parameters.Language}=eng";

            var markers = await markerService.ScrapeMarkersAsync(url);

            return new OkObjectResult($"GetMarkers function works! \n {JsonConvert.SerializeObject(markers)}");
        }
    }
}
