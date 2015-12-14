namespace Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class NewsCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NewsCategory()
        {
            News = new HashSet<News>();
        }

        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public long ParentID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<News> News { get; set; }
    }
}
