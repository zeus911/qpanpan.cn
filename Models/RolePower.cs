namespace Models
{
    public partial class RolePower
    {
        public long Id { get; set; }

        public long RoleId { get; set; }

        public long PowerId { get; set; }

        public virtual Power Power { get; set; }

        public virtual Role T_Roles { get; set; }
    }
}
