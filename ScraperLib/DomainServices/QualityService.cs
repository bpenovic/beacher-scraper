using System;
using System.Collections.Generic;
using System.Globalization;
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
using ScraperLib.Enums;

namespace ScraperLib.DomainServices
{
    public class QualityService : DomainBaseService, IQualityService
    {
        public QualityService(ScraperDbContext context, HttpClient client, IOptions<AppSettings> settings) : base(settings, client)
        {
        }

        public async Task<List<Quality>> ScrapeAndSaveQualityAsync(string url, IEnumerable<Marker> markers)
        {
            var qualities = new List<Quality>();

            if (markers != null)
            {
                //var tasks = markers.Select(marker => ScrapeQualityAsync(url, marker));
                //await Task.WhenAll(tasks);
                //qualities = tasks.SelectMany(t => t.Result).ToList();
                foreach (var marker in markers)
                {
                    var result = await ScrapeQualityAsync(url, marker);
                    qualities.AddRange(result);
                }
            }

            await SaveMarkerQualitiesAsync(qualities);
            return qualities;
        }
        public async Task<List<Quality>> ScrapeAndSaveQualityAsync(string url, Marker marker)
        {
            var qualities = await ScrapeQualityAsync(url, marker);
            await SaveMarkerQualitiesAsync(qualities);
            return qualities;
        }
        public async Task<IEnumerable<Quality>> GetQualitiesForMarkerAsync(int markerId)
        {
            var qualities = new List<Quality>();
            using (var dbContext = GetDbContext(false))
            {
                if (markerId > 0)
                    qualities = await dbContext.Qualities.Where(x => x.MarkerId == markerId).Select(Quality.Select)
                        .ToListAsync();

                return qualities;
            }
        }
        private async Task<List<Quality>> ScrapeQualityAsync(string url, Marker marker)
        {
            var qualities = new List<Quality>();
            if (marker != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                try
                {
                    var result = await HttpClient.GetStringAsync($"{url}&p_lok_id={marker.DataId}");
                    if (!string.IsNullOrEmpty(result))
                    {
                        var doc = new HtmlDocument();
                        doc.LoadHtml(result);
                        var tables = doc.DocumentNode.Descendants("table");

                        if (tables != null)
                            foreach (var table in tables)
                            {
                                var date = GetQualityMeasurementDate(table.Descendants("td").LastOrDefault());
                                if (DateTime.TryParseExact(date, "dd.MM.yyyy hh:mm", CultureInfo.CurrentCulture,
                                    DateTimeStyles.None, out var parsedDate))
                                {
                                    var mark = (int)GetQualityMark(table.Descendants("a").FirstOrDefault());
                                    var quality = new Quality
                                    {
                                        Date = parsedDate,
                                        Value = mark,
                                        MarkerId = marker.Id
                                    };
                                    qualities.Add(quality);
                                }
                            }
                    }
                }
                catch
                {
                    //igonored
                }
            }
            return qualities;
        }
        private async Task SaveMarkerQualitiesAsync(IEnumerable<Quality> qualities)
        {
            if (qualities != null)
            {
                var operationGuid = Guid.NewGuid();
                using (var dbContext = GetDbContext(false))
                {
                    var qualitiesDb = await dbContext.Qualities.ToListAsync();
                    foreach (var quality in qualities)
                    {
                        if (qualitiesDb != null && qualitiesDb.Count > 0)
                        {
                            var qualityExists = qualitiesDb.Any(x => x.Date == quality.Date && x.MarkerId == quality.MarkerId);
                            if (!qualityExists)
                                InsertQualityToContext(dbContext, quality);
                            else
                            {
                                dbContext.QualitiesForUpdate.Add(new Models.QualityForUpdate()
                                {
                                    Date = quality.Date,
                                    MarkerId = quality.MarkerId,
                                    Value = quality.Value,
                                    OperationGuid = operationGuid
                                });
                            }
                        }
                        else
                            InsertQualityToContext(dbContext, quality);
                    }

                    await dbContext.SaveChangesAsync();
                    await UpdateQualityAsync(dbContext, operationGuid);
                }
            }
        }
        private string GetQualityMeasurementDate(HtmlNode node)
        {
            var date = "";
            var dateNode = node?.FirstChild;

            if (dateNode != null)
                date = dateNode.InnerHtml;

            return date;
        }
        private QualityEnum GetQualityMark(HtmlNode node)
        {
            var mark = "";
            if (node != null)
            {
                var nodes = node.ChildNodes;
                foreach (var value in nodes)
                    if (!value.InnerHtml.Contains('+') && value.Name == "#text")
                        mark = value.InnerHtml.Replace(" ", string.Empty);
            }

            var enumKey = char.ToUpper(mark[0]) + mark.Substring(1);
            if (Enum.IsDefined(typeof(QualityEnum), enumKey))
                return (QualityEnum)Enum.Parse(typeof(QualityEnum), enumKey);

            return QualityEnum.Unknown;
        }
        private void InsertQualityToContext(ScraperDbContext dbContext, Quality quality)
        {
            dbContext.Qualities.Add(new Models.Quality
            {
                Date = quality.Date,
                MarkerId = quality.MarkerId,
                Value = quality.Value
            });
        }
        private async Task UpdateQualityAsync(ScraperDbContext dbContext, Guid operationGuid)
        {
            await dbContext.Database.ExecuteSqlCommandAsync($@"
                        UPDATE Q 
                        SET 
                            Value = tmp.Value
                        FROM Qualities Q
                        JOIN QualitiesForUpdate tmp on tmp.MarkerId = Q.MarkerId and tmp.Date = Q.Date and tmp.OperationGuid = {operationGuid}");
            await dbContext.Database.ExecuteSqlCommandAsync($"Delete from QualitiesForUpdate where OperationGuid = {operationGuid}");
        }
    }
}
