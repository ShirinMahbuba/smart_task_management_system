using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Tasks")]
    public class Task
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ProjectID { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? CreatedBy { get; set; }

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project? Project { get; set; }

        public virtual List<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public virtual List<TaskStep> TaskSteps { get; set; } = new List<TaskStep>();
        public virtual List<TaskDependency> TaskDependencies { get; set; } = new List<TaskDependency>();
        public virtual List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        public bool Deleted { get; set; }
        
        public string? Status { get; set; }
    }
}
