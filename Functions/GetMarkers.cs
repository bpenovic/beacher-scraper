using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scraper.Services;

namespace Srcaper.Functions
{
    public static class GetMarkers
    {
        [FunctionName("GetMarkers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var markerService = new MarkerService();
            //Get all markers
            var markers = await markerService.GetMarkersAsync("http://baltazar.izor.hr/plazepub/kakvoca_prikaz_xml9?p_god=2018&p_filter=&p_ciklus=-2&p_zup_id=&p_jezik=eng&p_grad=&pobjs=");


            return new OkObjectResult($"Scraper function works! \n {JsonConvert.SerializeObject(markers)}");
        }
    }
}
