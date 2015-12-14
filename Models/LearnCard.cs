namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class LearnCard
    {
        public long ID { get; set; }

        public long CourseID { get; set; }

        [Required]
        [StringLength(250)]
        public string CardNum { get; set; }

        public short ExpireDays { get; set; }

        [Required]
        [StringLength(16)]
        public string Password { get; set; }

        public DateTime CreateDateTime { get; set; }

        public long? FrontUserID { get; set; }

        public DateTime? ActiveDateTime { get; set; }

        public virtual Course Course { get; set; }

        public virtual FrontUser FrontUser { get; set; }
    }
}
