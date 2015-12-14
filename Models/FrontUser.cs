namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class FrontUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FrontUser()
        {
            CourseOrder = new HashSet<CourseOrder>();
            FrontUserCourse = new HashSet<FrontUserCourse>();
            LearnCard = new HashSet<LearnCard>();
        }

        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public DateTime RegDateTime { get; set; }

        public bool IsActive { get; set; }

        [StringLength(16)]
        public string PhoneNum { get; set; }

        [StringLength(16)]
        public string QQ { get; set; }

        [StringLength(50)]
        public string School { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseOrder> CourseOrder { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FrontUserCourse> FrontUserCourse { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LearnCard> LearnCard { get; set; }
    }
}
