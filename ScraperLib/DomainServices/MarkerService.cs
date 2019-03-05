using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ScraperLib.DAL;
using ScraperLib.DomainServices.Interfaces;
using ScraperLib.Enums;
using ScraperLib.DomainModels;
using ScraperLib.DomainModels.ParseModels;

namespace ScraperLib.DomainServices
{
    public class MarkerService : IMarkerService
    {
        private readonly HttpClient _client;
        private readonly ScraperDbContext _context;
        public MarkerService(ScraperDbContext context)
        {
            _client = new HttpClient();
            _context = context;
        }
        public async Task<IEnumerable<Marker>> ScrapeMarkersAsync(string url)
        {
            Console.WriteLine("\nFetching markers... \n");
            var markerList = new List<Marker>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var result = await _client.GetStringAsync(url);

            if (!string.IsNullOrEmpty(result))
            {
                XDocument doc = XDocument.Parse(result);
                var markers = doc.Descendants("marker").ToList();

                if (markers.Count > 0)
                {
                    foreach (var xMarker in markers)
                    {
                        var reader = new StringReader(xMarker.ToString());
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(MarkerParseModel));
                        var marker = (MarkerParseModel)xmlSerializer.Deserialize(reader);

                        markerList.Add(new Marker
                        {
                            Name = marker.Name,
                            City = marker.City,
                            DataId = marker.Id,
                            Latitude = marker.Latitude,
                            Longitude = marker.Longitude
                        });
                    }

                    await SaveMarkersAsync(markerList);
                }
                else
                    Console.WriteLine("Marker doesn't exist");
            }
            else
                Console.WriteLine("Result is empty");

            return markerList;
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
                            if (DateTime.TryParseExact(date, "dd.MM.yyyy hh:mm", CultureInfo.CurrentCulture,
                                DateTimeStyles.None, out var parsedDate))
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

        public async Task<IEnumerable<Marker>> GetMarkersAsync()
        {
            return await _context.Markers.Select(Marker.Select).ToListAsync();
        }

        public async Task<Marker> GetMarkerById(int id)
        {
            return await _context.Markers.Select(Marker.Select).FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task SaveMarkersAsync(IEnumerable<Marker> markers)
        {
            if (markers != null)
            {
                foreach (var marker in markers)
                {
                    _context.Markers.Add(new Models.Marker
                    {
                        City = marker.City,
                        Name = marker.Name,
                        Location = new Point(marker.Latitude, marker.Longitude) { SRID = Statics.Srid },
                        DataId = marker.DataId
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task SaveMarkerQualitiesAsync(int markerId, IEnumerable<Quality> qualities)
        {
            if (markerId > 0 && qualities != null)
            {
                foreach (var quality in qualities)
                {
                    _context.Qualities.Add(new Models.Quality
                    {
                        Date = quality.Date,
                        MarkerId = markerId,
                        Value = quality.Value
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        private string GetQualityMeasurementDate(HtmlNode node)
        {
            var date = "";
            if (node != null)
            {
                var dateNode = node.FirstChild;
                if (dateNode != null)
                    date = dateNode.InnerHtml;
            }

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