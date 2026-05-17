using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Tasks")]
    public class Task
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? ProjectID { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project? Project { get; set; }

        public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public virtual ICollection<TaskStep> TaskSteps { get; set; } = new List<TaskStep>();
        public virtual ICollection<TaskDependency> TaskDependencies { get; set; } = new List<TaskDependency>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
