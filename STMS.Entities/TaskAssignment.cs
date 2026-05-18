using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("TaskAssignments")]
    public class TaskAssignment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TaskID { get; set; }
        public int UserID { get; set; }
        public int AssignedBy { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}
