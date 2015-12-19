using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class IdentityCardNumberAttribution
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int IdentityCardNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Attribution { get; set; }
    }
}
