using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScraperLib.Models
{
    public class Marker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        public virtual ICollection<Quality> Qualities { get; set; }
    }
}
