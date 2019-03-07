using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace ScraperLib.Models
{
    public class Marker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Point Location { get; set; }
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        [Required]
        public int DataId { get; set; }
        public virtual ICollection<Quality> Qualities { get; set; }
        public virtual ICollection<Details> Details { get; set; }
    }
}
