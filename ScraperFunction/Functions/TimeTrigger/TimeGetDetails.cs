using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScrapeFunction.Containers;
using ScrapeFunction.Modules;
using ScraperLib;
using ScraperLib.DomainServices.Interfaces;

namespace ScraperFunction.Functions.TimeTrigger
{
    public static class TimeGetDetails
    {
        public static IServiceProvider Container = new ContainerBuilder()
            .RegisterModule(new CoreAppModule())
            .Build();

        //At 02:00 in every month.
        [FunctionName("TimeGetDetails")]
        public static async Task Run([TimerTrigger("0 2 * */1 *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TimeGetDetails trigger function executed at: {DateTime.Now}");

            var markerService = Container.GetRequiredService<IMarkerService>();
            var detailsService = Container.GetRequiredService<IDetailsService>();

            var url = $"{Endpoints.Details}?{Parameters.Season}=2018&{Parameters.Language}=eng";

            var markers = await markerService.GetMarkersAsync();
            await detailsService.ScrapeAndSaveDetailsAsync(url, markers);
        }
    }
}
