using System;
using System.Xml.Serialization;

namespace Scraper.Models
{
    [Serializable()]
    [XmlRoot(ElementName = "marker")]
    public class MarkerModel
    {
        [XmlAttribute("lsta")]
        public int Id { get; set; }
        [XmlAttribute("lat")]
        public double Latitude { get; set; }

        [XmlAttribute("lng")]
        public double Longitude { get; set; }

        [XmlAttribute("lpla")]
        public string Name { get; set; }
        [XmlAttribute("lgrad")]
        public string City { get; set; }
    }
}
