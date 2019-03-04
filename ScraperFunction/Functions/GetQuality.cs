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
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var quality = new List<Quality>();
            var markerService = Container.GetRequiredService<IMarkerService>();
            if (Int32.TryParse(req.Query["markerId"], out var markerId))
            {
                if (Int32.TryParse(req.Query["markerDataId"], out var markerDataId))
                {
                    var markerName = req.Query["markerName"];
                    var marker = new Marker()
                    {
                        Id = markerId,
                        Name = markerName,
                        DataId = markerDataId
                    };
                    quality = (await markerService.ScrapeQualityAsync("http://baltazar.izor.hr/plazepub/kakvoca_ispitivanja9?p_god=2018&p_ciklus=-2&p_jezik=eng&p_prikaz=,&p_cprikaz=,ci,",marker));
                }
            }
            else
            {
                var markers = await markerService.GetMarkersAsync();
                await markerService.ScrapeQualityAsync("http://baltazar.izor.hr/plazepub/kakvoca_ispitivanja9?p_god=2018&p_ciklus=-2&p_jezik=eng&p_prikaz=,&p_cprikaz=,ci,", markers);
            }

            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(quality)}");
        }
    }
}
