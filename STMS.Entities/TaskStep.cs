using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("TaskStep")]
    public class TaskStep
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? TaskID { get; set; }

        [Required]
        [StringLength(255)]
        public string StepTitle { get; set; } = null!;

        public bool IsCompleted { get; set; } = false;

        public int StepOrder { get; set; } = 0;

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}
