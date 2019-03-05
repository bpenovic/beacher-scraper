using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ScraperLib.DAL;
using ScraperLib.DomainServices.Interfaces;
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
        public async Task<IEnumerable<Marker>> GetMarkersAsync()
        {
            return await _context.Markers.Select(Marker.Select).ToListAsync();
        }
        public async Task<Marker> GetMarkerByIdAsync(int id)
        {
            return await _context.Markers.Select(Marker.Select).FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task SaveMarkersAsync(IEnumerable<Marker> markers)
        {
            if (markers != null)
            {
                var markersDb = await _context.Markers.ToListAsync();
                foreach (var marker in markers)
                {
                    if (markersDb != null)
                    {
                        var markerDb = markersDb.FirstOrDefault(x => x.DataId == marker.DataId);
                        if (markerDb is null)
                           InsertMarkerToContext(marker);
                        else
                        {
                            markerDb.DataId = marker.DataId;
                            markerDb.City = marker.City;
                            markerDb.Location = new Point(marker.Latitude, marker.Longitude) { SRID = Statics.Srid };
                            _context.Markers.Update(markerDb);
                        }
                    }
                    else
                        InsertMarkerToContext(marker);
                }

                await _context.SaveChangesAsync();
            }
        }
        private void InsertMarkerToContext(Marker marker)
        {
            _context.Markers.Add(new Models.Marker
            {
                City = marker.City,
                Name = marker.Name,
                Location = new Point(marker.Latitude, marker.Longitude) { SRID = Statics.Srid },
                DataId = marker.DataId
            });
        }
    }
}