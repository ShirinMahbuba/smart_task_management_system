using System.ComponentModel.DataAnnotations;

namespace STMS.Models
{
    public class ViewerDashboardModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int HighPriorityTasks { get; set; }
        public List<ViewerProjectSummaryModel> Projects { get; set; } = new();
        public List<ViewerTaskListItemModel> RecentTasks { get; set; } = new();
        public List<ViewerPostModel> RecentPosts { get; set; } = new();
    }

    public class ViewerProjectSummaryModel
    {
        public int ID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public decimal CompletionPercent { get; set; }
    }

    public class ViewerTaskListItemModel
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ViewerTaskDetailsModel : ViewerTaskListItemModel
    {
        public List<ViewerTaskStepModel> Steps { get; set; } = new();
        public List<ViewerCommentModel> Comments { get; set; } = new();
        public List<ViewerAttachmentModel> Attachments { get; set; } = new();
    }

    public class ViewerTaskStepModel
    {
        public int ID { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string PerformedByName { get; set; } = string.Empty;
    }

    public class ViewerCommentModel
    {
        public int ID { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ViewerAttachmentModel
    {
        public int ID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
    }

    public class ViewerPostModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ViewerPostEditModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;
    }

    public class ViewerReportModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CollaborationPosts { get; set; }
        public List<ViewerStatusCountModel> TasksByStatus { get; set; } = new();
        public List<ViewerPriorityCountModel> TasksByPriority { get; set; } = new();
        public List<ViewerProjectSummaryModel> ProjectProgress { get; set; } = new();
    }

    public class ViewerStatusCountModel
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ViewerPriorityCountModel
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
