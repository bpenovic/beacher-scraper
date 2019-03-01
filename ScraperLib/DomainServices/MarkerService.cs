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
using ScraperLib.DomainServices.Interfaces;
using ScraperLib.Enums;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices
{
    public class MarkerService : IMarkerService
    {
        private readonly HttpClient _client;

        public MarkerService()
        {
            _client = new HttpClient();
        }
        public async Task<IEnumerable<Marker>> GetMarkersAsync(string url)
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
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Marker));
                        var marker = (Marker)xmlSerializer.Deserialize(reader);
                        Console.WriteLine("{0} {1} {2} {3} {4}", marker.Id.ToString(), marker.Name, marker.City, marker.Longitude, marker.Longitude);
                        markerList.Add(marker);
                    }
                }
                else
                    Console.WriteLine("Marker doesn't exist");
            }
            else
                Console.WriteLine("Result is empty");

            return markerList;
        }

        public async Task<List<Quality>> GetQualityAsync(string url, Marker marker)
        {
            var data = new List<Quality>();
            if (marker != null)
            {
                Console.WriteLine($"\nFetching quality for {marker.Id} {marker.Name}... \n");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var result = await _client.GetStringAsync($"{url}&p_lok_id={marker.Id}");

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
                                data.Add(quality);
                            }
                        }
                    }
                    else
                        Console.WriteLine("Td doesn't exist");
                }
                else
                    Console.WriteLine("Result is empty");
            }
            return data;
        }

        public async Task<Profile> GetDetailsAsync(string url, Marker marker)
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