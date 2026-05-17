using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Comments")]
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? UserID { get; set; }
        public int? TaskID { get; set; }
        public int? PostID { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }

        [ForeignKey("PostID")]
        public virtual Post? Post { get; set; }
    }
}
