using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("Users")]
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Viewer";

        public bool IsActive { get; set; } = true;
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }

        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
