namespace Models
{
    using System.Data.Entity;

    public partial class qds173643455_db : DbContext
    {
        public qds173643455_db()
            : base("name=qds173643455_db")
        {
        }

        public virtual DbSet<AdminOperationLog> AdminOperationLog { get; set; }
        public virtual DbSet<AdminUserRole> AdminUserRole { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<Chapter> Chapter { get; set; }
        public virtual DbSet<CourseOrder> CourseOrder { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<FrontUserActiveCode> FrontUserActiveCode { get; set; }
        public virtual DbSet<FrontUserCourse> FrontUserCourse { get; set; }
        public virtual DbSet<FrontUser> FrontUser { get; set; }
        public virtual DbSet<LearnCard> LearnCard { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsCategory> NewsCategory { get; set; }
        public virtual DbSet<Power> Power { get; set; }
        public virtual DbSet<RolePower> RolePower { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Segment> Segment { get; set; }
        public virtual DbSet<IdentityCardNumberAttribution> IdentityCardNumberAttribution { get; set; }
        public virtual DbSet<Geographical> Geographical { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<LicensePlate> LicensePlate { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminOperationLog>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AdminUser>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<AdminUser>()
                .HasMany(e => e.AdminOperationLog)
                .WithRequired(e => e.AdminUser)
                .HasForeignKey(e => e.AdminUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdminUser>()
                .HasMany(e => e.AdminUserRole)
                .WithRequired(e => e.AdminUser)
                .HasForeignKey(e => e.AdminUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Chapter>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Chapter>()
                .HasMany(e => e.Segment)
                .WithRequired(e => e.T_Chapters)
                .HasForeignKey(e => e.ChapterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.Chapter)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseOrder)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.FrontUserCourse)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.LearnCard)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FrontUserActiveCode>()
                .Property(e => e.ActiveCode)
                .IsUnicode(false);

            modelBuilder.Entity<FrontUser>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<FrontUser>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<FrontUser>()
                .Property(e => e.PhoneNum)
                .IsUnicode(false);

            modelBuilder.Entity<FrontUser>()
                .Property(e => e.QQ)
                .IsUnicode(false);

            modelBuilder.Entity<FrontUser>()
                .HasMany(e => e.CourseOrder)
                .WithRequired(e => e.FrontUser)
                .HasForeignKey(e => e.FrontUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FrontUser>()
                .HasMany(e => e.FrontUserCourse)
                .WithRequired(e => e.FrontUser)
                .HasForeignKey(e => e.FrontUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FrontUser>()
                .HasMany(e => e.LearnCard)
                .WithOptional(e => e.FrontUser)
                .HasForeignKey(e => e.FrontUserID);

            modelBuilder.Entity<LearnCard>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<News>()
                .Property(e => e.Article)
                .IsUnicode(false);

            modelBuilder.Entity<NewsCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<NewsCategory>()
                .HasMany(e => e.News)
                .WithRequired(e => e.NewsCategory)
                .HasForeignKey(e => e.NewsCategoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Power>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Power>()
                .HasMany(e => e.RolePower)
                .WithRequired(e => e.Power)
                .HasForeignKey(e => e.PowerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.AdminUserRole)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.RolePower)
                .WithRequired(e => e.T_Roles)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Segment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Segment>()
                .Property(e => e.VideoCode)
                .IsUnicode(false);

            modelBuilder.Entity<Segment>()
                .Property(e => e.Note)
                .IsUnicode(false);



            modelBuilder.Entity<IdentityCardNumberAttribution>()
                .Property(e => e.Attribution)
                .IsUnicode(false);

            modelBuilder.Entity<Geographical>()
                .Property(e => e.Name)
                .IsUnicode(false);
            modelBuilder.Entity<Geographical>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<Region>()
                .Property(e => e.FullName)
                .IsUnicode(false);
            modelBuilder.Entity<Region>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<LicensePlate>()
                .Property(e => e.Code)
                .IsUnicode(false);
            modelBuilder.Entity<LicensePlate>()
                .Property(e => e.Area)
                .IsUnicode(false);
        }
    }
}
