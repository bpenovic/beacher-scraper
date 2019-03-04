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
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;

namespace ScrapeFunction.Functions
{
    public static class GetMarkerDetails
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("GetMarkerDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var markerDetails = new Profile();
            var markerService = Container.GetRequiredService<IMarkerService>();

            if (Int32.TryParse(req.Query["markerId"], out int markerId))
            {
                var markerName = req.Query["markerName"];
                var marker = new Marker
                {
                    Id = markerId,
                    Name = markerName
                };
                markerDetails = await markerService.ScrapeDetailsAsync("http://baltazar.izor.hr/plazepub/profil_plaze?psez=2018&p_jezik=eng", marker);
            }

            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(markerDetails)}");
        }
    }
}

