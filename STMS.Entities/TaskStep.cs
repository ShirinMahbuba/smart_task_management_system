using System;
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

        public string? Status { get; set; }

        public DateTime DateTime { get; set; }

        public int PerformedBy { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task? Task { get; set; }
    }
}