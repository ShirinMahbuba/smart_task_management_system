// TaskDTO.cs
namespace STMS.DTOs
{
    public class TaskDTO
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string? CurrentStatus { get; set; }
    }
}

