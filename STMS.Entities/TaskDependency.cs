using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("TaskDependencies")]
    public class TaskDependency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? TaskID { get; set; }
        public int? DependsOnTaskID { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}
