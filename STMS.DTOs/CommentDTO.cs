using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DTOs
{
    public class CommentDTO
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string? CommentText { get; set; }
    }
}