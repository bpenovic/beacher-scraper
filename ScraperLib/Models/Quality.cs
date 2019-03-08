using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScraperLib.Models
{
    public class Quality
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Value { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int MarkerId { get; set; }
        [ForeignKey(nameof(MarkerId))]
        public virtual Marker Marker { get; set; }
    }

    public class QualityForUpdate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Value { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int MarkerId { get; set; }
        public Guid OperationGuid { get; set; }
    }
}
