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
using ScraperLib.DomainServices.Interfaces;
using ScraperLib.Models;
using Details = ScraperLib.DomainModels.Details;
using Marker = ScraperLib.DomainModels.Marker;

namespace ScraperLib.DomainServices
{
    public class DetailsService : DomainBaseService, IDetailsService
    {
        public DetailsService(ScraperDbContext context, HttpClient client, IOptions<AppSettings> settings) : base(settings, client)
        {
        }

        public async Task<List<Details>> ScrapeAndSaveDetailsAsync(string url, IEnumerable<Marker> markers)
        {
            var details = new List<Details>();
            if (markers != null)
            {
                //var tasks = markers.Select(marker => ScrapeDetailsAsync(url, marker));
                //await Task.WhenAll(tasks);
                //details = tasks.Select(t => t.Result).ToList();
                foreach (var marker in markers)
                {
                    var result = await ScrapeDetailsAsync(url, marker);
                    details.Add(result);
                }
            }

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
                using (var dbContext = GetDbContext(true))
                {
                    var detailsDb = await dbContext.Details.ToListAsync();
                    if (detailsDb != null)
                    {
                        var detailDb = detailsDb.FirstOrDefault(x => x.MarkerId == detail.MarkerId);
                        if (detailDb is null)
                            InsertDetailToContext(dbContext, detail);
                        else
                        {
                            UpdateDetailToContext(dbContext, detailDb, detail);                         
                        }
                    }
                    else
                        InsertDetailToContext(dbContext, detail);

                    await dbContext.SaveChangesAsync();
                }
            }
        }
        private async Task SaveMarkerDetailsAsync(List<Details> details)
        {
            if (details != null)
            {
                using (var dbContext = GetDbContext(true))
                {
                    var operationGuid = Guid.NewGuid();
                    var detailsDb = await dbContext.Details.ToListAsync();
                    foreach (var detail in details)
                    {
                        if (detailsDb != null)
                        {
                            var detailExist = detailsDb.Any(x => x.MarkerId == detail.MarkerId);
                            if (!detailExist)
                                InsertDetailToContext(dbContext, detail);
                            else
                            {
                                dbContext.DetailsForUpdate.Add(new DetailsForUpdate()
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
                                    OperationGuid = operationGuid,
                                    MarkerId = detail.MarkerId
                                });
                            }
                        }
                        else
                            InsertDetailToContext(dbContext, detail);
                    }

                    await dbContext.SaveChangesAsync();
                    await UpdateDetailAsync(dbContext, operationGuid);
                }
            }
        }
        private async Task<Details> ScrapeDetailsAsync(string url, Marker marker)
        {
            var detail = new Details();
            if (marker != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                try
                {
                    var result = await HttpClient.GetStringAsync($"{url}&plok={marker.DataId}");

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
                }
                catch
                {
                    // ignored
                }
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
        private void InsertDetailToContext(ScraperDbContext dbContext, Details detail)
        {
            dbContext.Details.Add(new Models.Details
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
        private void UpdateDetailToContext(ScraperDbContext dbContext, Models.Details detailDb, Details detail)
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
            dbContext.Details.Update(detailDb);
        }
        private async Task UpdateDetailAsync(ScraperDbContext dbContext, Guid operationGuid)
        {
            await dbContext.Database.ExecuteSqlCommandAsync($@"
                        UPDATE D
                        SET 
                            Type = tmp.Type,
                            SurfaceType = tmp.Type,
                            Vegetation = tmp.Vegetation,
                            Shape = tmp.Shape,
                            AverageTemperature = tmp.AverageTemperature,
                            MaxSalinity = tmp.MaxSalinity,
                            MinSalinity = tmp.MinSalinity,
                            Wind = tmp.Wind,
                            Width = tmp.Width,
                            Length = tmp.Length
                        FROM Details D
                        JOIN DetailsForUpdate tmp on tmp.MarkerId = D.MarkerId and tmp.OperationGuid = {operationGuid}");
            await dbContext.Database.ExecuteSqlCommandAsync($"Delete from DetailsForUpdate where OperationGuid = {operationGuid}");
        }
    }
}
