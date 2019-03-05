using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using ScraperLib.DAL;
using ScraperLib.DomainModels;
using ScraperLib.DomainServices.Interfaces;
using ScraperLib.Enums;

namespace ScraperLib.DomainServices
{
    public class QualityService : IQualityService
    {
        private readonly HttpClient _client;
        private readonly ScraperDbContext _context;
        public QualityService(ScraperDbContext context)
        {
            _client = new HttpClient();
            _context = context;
        }

        public async Task ScrapeQualityAsync(string url, IEnumerable<Marker> markers)
        {
            if (markers != null)
                foreach (var marker in markers)
                    await ScrapeQualityAsync(url, marker);
        }
        public async Task<List<Quality>> ScrapeQualityAsync(string url, Marker marker)
        {
            var qualities = new List<Quality>();
            if (marker != null)
            {
                Console.WriteLine($"\nFetching quality for {marker.Id} {marker.Name}... \n");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var result = await _client.GetStringAsync($"{url}&p_lok_id={marker.DataId}");

                if (!string.IsNullOrEmpty(result))
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(result);
                    var tables = doc.DocumentNode.Descendants("table");

                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            var date = GetQualityMeasurementDate(table.Descendants("td").LastOrDefault());
                            if (DateTime.TryParseExact(date, "dd.MM.yyyy hh:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsedDate))
                            {
                                var mark = (int)GetQualityMark(table.Descendants("a").FirstOrDefault());
                                var quality = new Quality
                                {
                                    Date = parsedDate,
                                    Value = mark
                                };
                                qualities.Add(quality);
                            }
                        }

                        await SaveMarkerQualitiesAsync(marker.Id, qualities);
                    }
                    else
                        Console.WriteLine("Td doesn't exist");
                }
                else
                    Console.WriteLine("Result is empty");
            }
            return qualities;
        }
        public async Task<IEnumerable<Quality>> GetQualitiesForMarkerAsync(int markerId)
        {
            var qualities = new List<Quality>();
            if (markerId > 0)
                qualities = await _context.Qualities.Where(x => x.MarkerId == markerId).Select(Quality.Select).ToListAsync();

            return qualities;
        }

        private async Task SaveMarkerQualitiesAsync(int markerId, IEnumerable<Quality> qualities)
        {
            if (markerId > 0 && qualities != null)
            {
                var qualitiesDb = await _context.Qualities.Where(x => x.MarkerId == markerId).ToListAsync();
                foreach (var quality in qualities)
                {
                    if (qualitiesDb != null)
                    {
                        var qualityDb = qualitiesDb.FirstOrDefault(x => x.Date == quality.Date);
                        if (qualityDb is null)
                            InsertQualityToContext(markerId, quality);
                        else
                        {
                            qualityDb.Date = quality.Date;
                            qualityDb.Value = quality.Value;
                            qualityDb.MarkerId = quality.MarkerId;
                            _context.Qualities.Update(qualityDb);
                        }
                    }
                    else
                        InsertQualityToContext(markerId, quality);
                }

                await _context.SaveChangesAsync();
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
        private void InsertQualityToContext(int markerId, Quality quality)
        {
            _context.Qualities.Add(new Models.Quality
            {
                Date = quality.Date,
                MarkerId = markerId,
                Value = quality.Value
            });
        }
    }
}
