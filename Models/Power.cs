namespace Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Power
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Power()
        {
            RolePower = new HashSet<RolePower>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolePower> RolePower { get; set; }
    }
}
