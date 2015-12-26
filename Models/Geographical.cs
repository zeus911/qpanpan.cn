using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public sealed class Geographical
    {
        public Geographical()
        {
            Regions = new HashSet<Region>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Region> Regions { get; set; }
    }
}
