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
using Scraper.Models;

namespace Scraper.Functions
{
    public static class GetMarkerDetails
    {
        [FunctionName("GetMarkerDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var markerDetails = new ProfileModel();

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
                markerDetails = await markerService.GetDetailsAsync("http://baltazar.izor.hr/plazepub/profil_plaze?psez=2018&p_jezik=eng", marker);

            }

            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(markerDetails)}");
        }
    }
}

