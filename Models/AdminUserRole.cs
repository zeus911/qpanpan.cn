namespace Models
{
    public partial class AdminUserRole
    {
        public long Id { get; set; }

        public long AdminUserId { get; set; }

        public long RoleId { get; set; }

        public virtual AdminUser AdminUser { get; set; }

        public virtual Role Role { get; set; }
    }
}
