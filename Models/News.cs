namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class News
    {
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Article { get; set; }

        public DateTime PostDateTime { get; set; }

        public long NewsCategoryID { get; set; }

        public virtual NewsCategory NewsCategory { get; set; }
    }
}
