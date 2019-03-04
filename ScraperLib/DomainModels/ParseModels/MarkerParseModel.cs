using System;
using System.Xml.Serialization;

namespace ScraperLib.DomainModels.ParseModels
{
    [Serializable()]
    [XmlRoot(ElementName = "marker")]
    public class MarkerParseModel
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
