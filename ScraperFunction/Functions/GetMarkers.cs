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
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions
{

    public static class GetMarkers
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("GetMarkers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var markerService = Container.GetRequiredService<IMarkerService>();
            var markers = await markerService.ScrapeMarkersAsync("http://baltazar.izor.hr/plazepub/kakvoca_prikaz_xml9?p_god=2018&p_filter=&p_ciklus=-2&p_zup_id=&p_jezik=eng&p_grad=&pobjs=");

            return new OkObjectResult($"Scraper function works! \n {JsonConvert.SerializeObject(markers)}");
        }
    }
}
