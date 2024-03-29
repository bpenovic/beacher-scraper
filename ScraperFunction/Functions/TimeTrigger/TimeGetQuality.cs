using System;
using System.Threading.Tasks;
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
    public static class TimeGetQuality
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        //At 00:00 on every 7th day-of-week.
        [FunctionName("TimeGetQuality")]
        public static async Task Run([TimerTrigger("0 0 * * */7")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TimeGetQuality trigger function executed at: {DateTime.Now}");

            var qualityService = Container.GetRequiredService<IQualityService>();
            var markerService = Container.GetRequiredService<IMarkerService>();

            var url = $"{Endpoints.Quality}?{Parameters.Year}=2018&{Parameters.Cycle}=-2&{Parameters.Language}=eng&{Parameters.View}=,&{Parameters.CycleView}=,ci,";
            var markers = await markerService.GetMarkersAsync();
            var result = await qualityService.ScrapeAndSaveQualityAsync(url, markers);
            log.LogInformation($"{JsonConvert.SerializeObject(result)}");
        }
    }
}
