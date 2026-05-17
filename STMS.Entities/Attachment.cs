using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Attachments")]
    public class Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? TaskID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [StringLength(500)]
        public string? FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public int? UploadedBy { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}
