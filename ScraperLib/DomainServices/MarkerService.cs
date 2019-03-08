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
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using ScraperLib.DAL;
using ScraperLib.DomainServices.Interfaces;
using ScraperLib.DomainModels;
using ScraperLib.DomainModels.ParseModels;

namespace ScraperLib.DomainServices
{
    public class MarkerService : DomainBaseService, IMarkerService
    {
        private readonly HttpClient _client;
        private readonly ScraperDbContext _context;
        public MarkerService(ScraperDbContext context, IOptions<AppSettings> settings) : base(settings)
        {
            _client = new HttpClient();
            _context = context;
            _context.Database.SetCommandTimeout(50);
        }

        public async Task<IEnumerable<Marker>> ScrapeAndSaveMarkersAsync(string url)
        {
            var markers = await ScrapeMarkersAsync(url);
            return markers;
        }

        private async Task<IEnumerable<Marker>> ScrapeMarkersAsync(string url)
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
            using (var dbContext = GetDbContext(true))
            {
                return await dbContext.Markers.Select(Marker.Select).ToListAsync();
            }
        }
        public async Task<Marker> GetMarkerByIdAsync(int id)
        {
            using (var dbContext = GetDbContext(true))
            {
                return await dbContext.Markers.Select(Marker.Select).FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        private async Task SaveMarkersAsync(IEnumerable<Marker> markers)
        {
            if (markers != null)
            {
                var operationGuid = Guid.NewGuid();
                using (var dbContext = GetDbContext(true))
                {
                    var markersDb = await dbContext.Markers.ToListAsync();
                    foreach (var marker in markers)
                    {
                        if (markersDb != null)
                        {
                            var markerExist = markersDb.Any(x => x.DataId == marker.DataId);
                            if (!markerExist)
                                InsertMarkerToContext(dbContext, marker);
                            else
                            {
                                dbContext.MarkersForUpdate.Add(new Models.MarkerForUpdate
                                {
                                    Name = marker.Name,
                                    DataId = marker.DataId,
                                    City = marker.City,
                                    Location = new Point(marker.Latitude, marker.Longitude) { SRID = Statics.Srid },
                                    OperationGuid = operationGuid
                                });
                            }
                        }
                        else
                            InsertMarkerToContext(dbContext, marker);
                        await dbContext.SaveChangesAsync();
                        await UpdateMarkerAsync(dbContext, operationGuid);
                    }
                }
            }
        }
        private void InsertMarkerToContext(ScraperDbContext dbContext, Marker marker)
        {
            dbContext.Markers.Add(new Models.Marker
            {
                City = marker.City,
                Name = marker.Name,
                Location = new Point(marker.Latitude, marker.Longitude) { SRID = Statics.Srid },
                DataId = marker.DataId
            });
        }

        private async Task UpdateMarkerAsync(ScraperDbContext dbContext, Guid operationGuid)
        {
            await dbContext.Database.ExecuteSqlCommandAsync($@"
                        UPDATE M
                        SET 
                            City = tmp.City,
                            Name = tmp.Name,
                            Location = tmp.Location,
                            DataId = tmp.DataId
                        FROM Markers M
                        JOIN MarkersForUpdate tmp on tmp.DataId = M.DataId and tmp.OperationGuid = {operationGuid}");
            await dbContext.Database.ExecuteSqlCommandAsync($"Delete from MarkersForUpdate where OperationGuid = {operationGuid}");
        }
    }
}