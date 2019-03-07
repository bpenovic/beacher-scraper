using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
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
            _context.Database.SetCommandTimeout(50);
        }

        public async Task<List<Details>> ScrapeAndSaveDetailsAsync(string url, IEnumerable<Marker> markers)
        {
            var details = new List<Details>();
            if (markers != null)
                foreach (var marker in markers)
                    details.Add(await ScrapeDetailsAsync(url, marker));

            await SaveMarkerDetailsAsync(details);
            return details;
        }
        public async Task<Details> ScrapeAndSaveDetailsAsync(string url, Marker marker)
        {
            var details = await ScrapeDetailsAsync(url, marker);
            await SaveMarkerDetailAsync(details);
            return details;
        }

        private async Task SaveMarkerDetailAsync(Details detail)
        {
            if (detail != null)
            {
                var detailsDb = await _context.Details.ToListAsync();
                if (detailsDb != null)
                {
                    var detailDb = detailsDb.FirstOrDefault(x => x.MarkerId == detail.MarkerId);
                    if (detailDb is null)
                        InsertDetailToContext(detail);
                    else
                        UpdateDetailToContext(detailDb, detail);
                }
                else
                    InsertDetailToContext(detail);

                await _context.SaveChangesAsync();
            }
        }

        private async Task SaveMarkerDetailsAsync(List<Details> details)
        {
            if (details != null)
            {
                var detailsDb = await _context.Details.ToListAsync();
                foreach (var detail in details)
                {
                    if (detailsDb != null)
                    {
                        var detailDb = detailsDb.FirstOrDefault(x => x.MarkerId == detail.MarkerId);
                        if (detailDb is null)
                            InsertDetailToContext(detail);
                        else
                            UpdateDetailToContext(detailDb, detail);
                    }
                    else
                        InsertDetailToContext(detail);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task<Details> ScrapeDetailsAsync(string url, Marker marker)
        {
            var detail = new Details();
            if (marker != null)
            {
                Console.WriteLine($"\nFetching details for {marker.Id} {marker.Name}... \n");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var result = await _client.GetStringAsync($"{url}&plok={marker.DataId}");

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
                        {
                            AddAttribute(tr, detail);
                            detail.MarkerId = marker.Id;
                        }
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

                        if (key.StartsWith(Statics.Shape))
                            details.Shape = value;

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
        private void InsertDetailToContext(Details detail)
        {
            _context.Details.Add(new Models.Details
            {
                Type = detail.Type,
                SurfaceType = detail.SurfaceType,
                Vegetation = detail.Vegetation,
                Shape = detail.Shape,
                AverageTemperature = detail.AverageTemperature,
                MaxSalinity = detail.MaxSalinity,
                MinSalinity = detail.MinSalinity,
                Wind = detail.Wind,
                Width = detail.Width,
                Length = detail.Length,
                MarkerId = detail.MarkerId
            });
        }

        private void UpdateDetailToContext(Models.Details detailDb, Details detail)
        {
            detailDb.Type = detail.Type;
            detailDb.SurfaceType = detail.SurfaceType;
            detailDb.Vegetation = detail.Vegetation;
            detailDb.Shape = detail.Shape;
            detailDb.AverageTemperature = detail.AverageTemperature;
            detailDb.MaxSalinity = detail.MaxSalinity;
            detailDb.MinSalinity = detail.MinSalinity;
            detailDb.Wind = detail.Wind;
            detailDb.Width = detail.Width;
            detailDb.Length = detail.Length;
            _context.Details.Update(detailDb);
        }
    }
}
