using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainServices.Interfaces;

namespace ScraperFunction.Functions.TimeTrigger
{
    public static class TimeGetMarkers
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        [FunctionName("TimeGetMarkers")]
        public static async Task Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TimeGetMarkers trigger function executed at: {DateTime.Now}");
            var markerService = Container.GetRequiredService<IMarkerService>();
            var url = $"{Endpoints.Markers}?{Parameters.Year}=2018&{Parameters.Filter}=&{Parameters.Cycle}=-2&{Parameters.Language}=eng";

            await markerService.ScrapeAndSaveMarkersAsync(url);
        }
    }
}
