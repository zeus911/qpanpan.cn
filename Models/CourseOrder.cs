namespace Models
{
    using System;

    public partial class CourseOrder
    {
        public long ID { get; set; }

        public long CourseID { get; set; }

        public long FrontUserID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? PayDateTime { get; set; }

        public bool IsPayed { get; set; }

        public virtual Course Course { get; set; }

        public virtual FrontUser FrontUser { get; set; }
    }
}
