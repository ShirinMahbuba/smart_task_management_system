using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("TaskStep")]
    public class TaskStep
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TaskID { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime DateTime { get; set; } = DateTime.Now;
        public int PerformedBy { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}
