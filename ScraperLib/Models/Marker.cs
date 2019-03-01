using System;
using System.Xml.Serialization;

namespace ScraperLib.Models
{
    public class Marker
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }
}
