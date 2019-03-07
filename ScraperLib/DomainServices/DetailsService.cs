using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using ScraperLib.DAL;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices
{
    public class DetailsService : DomainBaseService
    {
        private readonly HttpClient _client;
        private readonly ScraperDbContext _context;
        public DetailsService(ScraperDbContext context, IOptions<AppSettings> settings) : base(settings)
        {
            _client = new HttpClient();
            _context = context;
        }

        public async Task<Profile> ScrapeDetailsAsync(string url, Marker marker)
        {
            var detail = new Profile();
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

        private void AddAttribute(HtmlNode node, Profile profile)
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
                            profile.Type = value;

                        if (key.StartsWith(Statics.AverageTemperature))
                            profile.AverageTemperature = double.Parse(value);

                        if (key.StartsWith(Statics.Length))
                            profile.Length = double.Parse(value);

                        if (key.StartsWith(Statics.Width))
                            profile.Width = double.Parse(value);

                        if (key.StartsWith(Statics.MaxSalinity))
                            profile.MaxSalinity = double.Parse(value);

                        if (key.StartsWith(Statics.MinSalinity))
                            profile.MinSalinity = double.Parse(value);

                        if (key.StartsWith(Statics.SurfaceType))
                            profile.SurfaceType = value;

                        if (key.StartsWith(Statics.Vegetation))
                            profile.Vegetation = value;

                        if (key.StartsWith(Statics.Wind))
                            profile.Wind = value;
                    }
                }
            }
        }
    }
}
