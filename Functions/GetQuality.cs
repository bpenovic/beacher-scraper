using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scraper.Models;
using Scraper.Services;
using System.Collections.Generic;

namespace Scraper.Functions
{
    public static class GetQuality
    {
        [FunctionName("GetQuality")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var quality = new List<QualityModel>();

            if (Int32.TryParse(req.Query["markerId"], out int markerId))
            {
                // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                // dynamic data = JsonConvert.DeserializeObject(requestBody);
                // var markerId = data?.markerId;
                // var markerName = data?.markerName;

                var markerName = req.Query["markerName"];
                var marker = new MarkerModel()
                {
                    Id = markerId,
                    Name = markerName
                };

                var markerService = new MarkerService();
                //Get quality of marker
                quality = (await markerService.GetQualityAsync("http://baltazar.izor.hr/plazepub/kakvoca_ispitivanja9?p_god=2018&p_ciklus=-2&p_jezik=eng&p_prikaz=,&p_cprikaz=,ci,", marker));

            }

            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(quality)}");
        }
    }
}
