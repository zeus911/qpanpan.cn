namespace Models
{
    using System;

    public partial class FrontUserCourse
    {
        public long ID { get; set; }

        public long CourseID { get; set; }

        public DateTime ExpireDateTime { get; set; }

        public long FrontUserID { get; set; }

        public virtual Course Course { get; set; }

        public virtual FrontUser FrontUser { get; set; }
    }
}
