namespace Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class Segment
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public long SeqNo { get; set; }

        [Required]
        public string VideoCode { get; set; }

        public string Note { get; set; }

        public long ChapterId { get; set; }

        public virtual Chapter T_Chapters { get; set; }
    }
}
