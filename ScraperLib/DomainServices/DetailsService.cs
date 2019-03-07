using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using ScraperLib.DAL;
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;

namespace ScraperLib.DomainServices
{
    public class DetailsService : DomainBaseService, IDetailsService
    {
        private readonly HttpClient _client;
        private readonly ScraperDbContext _context;
        public DetailsService(ScraperDbContext context, IOptions<AppSettings> settings) : base(settings)
        {
            _client = new HttpClient();
            _context = context;
        }

        public async Task<List<Details>> ScrapeAndSaveDetailsAsync(string url, IEnumerable<Marker> markers)
        {
            var details = new List<Details>();
            if (markers != null)
                foreach (var marker in markers)
                    details.Add(await ScrapeDetailsAsync(url, marker));

            return details;
        }
        public async Task<Details> ScrapeAndSaveDetailsAsync(string url, Marker marker)
        {
            var details = await ScrapeDetailsAsync(url, marker);
            return details;
        }

        private async Task<Details> ScrapeDetailsAsync(string url, Marker marker)
        {
            var detail = new Details();
            if (marker != null)
            {
                Console.WriteLine($"\nFetching details for {marker.Id} {marker.Name}... \n");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var result = await _client.GetStringAsync($"{url}&plok={marker.Id}");

                if (!string.IsNullOrEmpty(result))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(result);

                    var tables = doc.DocumentNode.Descendants("table");
                    var dataTable = tables?.ElementAt(1);

                    if (dataTable != null)
                    {
                        var trs = dataTable.Descendants("tr");
                        foreach (var tr in trs)
                            AddAttribute(tr, detail);
                    }
                }
                else
                    Console.WriteLine("Result is empty");
            }

            return detail;
        }

        private void AddAttribute(HtmlNode node, Details details)
        {
            if (node != null && !string.IsNullOrEmpty(node.InnerHtml))
            {
                var childs = node.Descendants("td");
                if (childs != null && childs.Any())
                {
                    var key = childs.First()?.InnerHtml;
                    var value = childs.Last()?.InnerHtml;

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        if (key.StartsWith(Statics.Type))
                            details.Type = value;

                        if (key.StartsWith(Statics.AverageTemperature))
                            details.AverageTemperature = double.Parse(value);

                        if (key.StartsWith(Statics.Length))
                            details.Length = double.Parse(value);

                        if (key.StartsWith(Statics.Width))
                            details.Width = double.Parse(value);

                        if (key.StartsWith(Statics.MaxSalinity))
                            details.MaxSalinity = double.Parse(value);

                        if (key.StartsWith(Statics.MinSalinity))
                            details.MinSalinity = double.Parse(value);

                        if (key.StartsWith(Statics.SurfaceType))
                            details.SurfaceType = value;

                        if (key.StartsWith(Statics.Vegetation))
                            details.Vegetation = value;

                        if (key.StartsWith(Statics.Wind))
                            details.Wind = value;
                    }
                }
            }
        }
    }
}
