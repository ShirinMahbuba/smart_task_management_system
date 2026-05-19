using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DTOs
{
    public class AttachmentDTO
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int UploadedBy { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}