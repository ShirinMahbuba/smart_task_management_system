using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Attachments")]
    public class Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TaskID { get; set; }
        public int UploadedBy { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [StringLength(500)]
        public string? FilePath { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}
