using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class LicensePlate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        public string Area { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
    }
}
