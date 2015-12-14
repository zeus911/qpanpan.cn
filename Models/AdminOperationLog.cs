namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class AdminOperationLog
    {
        public long Id { get; set; }

        public long AdminUserId { get; set; }

        public DateTime CreateDateTime { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual AdminUser AdminUser { get; set; }
    }
}
