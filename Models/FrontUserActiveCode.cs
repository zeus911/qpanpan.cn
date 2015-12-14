namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class FrontUserActiveCode
    {
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(40)]
        public string ActiveCode { get; set; }

        public DateTime Expires { get; set; }
    }
}
