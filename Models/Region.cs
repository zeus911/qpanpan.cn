using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Region
    {
        public Region()
        {
            LicensePlates = new HashSet<LicensePlate>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(4)]
        [Required]
        public string Name { get; set; }

        [Required]
        public int GeographicalId { get; set; }

        [ForeignKey("GeographicalId")]
        public virtual Geographical Geographical { get; set; }

        public ICollection<LicensePlate> LicensePlates { get; set; }
    }
}
