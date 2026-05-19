using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DTOs
{
    public class TaskStepDTO
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string? Status { get; set; }
        public int PerformedBy { get; set; }
    }
}

