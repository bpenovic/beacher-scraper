using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScraperLib.Models
{
    public class Details
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Type { get; set; }
        [StringLength(50)]
        public string SurfaceType { get; set; }
        [StringLength(100)]
        public string Vegetation { get; set; }
        [StringLength(100)]
        public string Shape { get; set; }
        public double AverageTemperature { get; set; }
        public double MaxSalinity { get; set; }
        public double MinSalinity { get; set; }
        [StringLength(50)]
        public string Wind { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public int MarkerId { get; set; }

        [ForeignKey(nameof(MarkerId))]
        public virtual Marker Marker { get; set; }
    }
}
