using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("TaskAssignments")]
    public class TaskAssignment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? TaskID { get; set; }
        public int? UserID { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}
