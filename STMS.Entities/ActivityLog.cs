using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STMS.Entities
{
    [Table("ActivityLogs")]
    public class ActivityLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? UserID { get; set; }

        [StringLength(255)]
        public string? Action { get; set; }

        public DateTime LogTime { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
}

