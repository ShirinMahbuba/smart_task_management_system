using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
