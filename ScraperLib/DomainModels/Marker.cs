using System;
using System.Linq.Expressions;

namespace ScraperLib.DomainModels
{
    public class Marker
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public static Expression<Func<Models.Marker, Marker>> Select => x => new Marker()
        {
            Id = x.Id,
            Name = x.Name,
            City = x.City,
            DataId = x.DataId,
            Latitude = x.Location.X,
            Longitude = x.Location.Y
        };
    }
}
