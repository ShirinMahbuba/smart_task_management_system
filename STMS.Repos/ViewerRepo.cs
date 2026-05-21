using Microsoft.EntityFrameworkCore;
using STMS.Data;
using STMS.Entities;
using STMS.Shared;
using TaskEntity = STMS.Entities.Task;

namespace STMS.Repos
{
    // ── View Models ──────────────────────────────────────────────────────────
    public class ViewerDashboardModel
    {
        public int TotalProjects   { get; set; }
        public int TotalTasks      { get; set; }
        public int PendingTasks    { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks  { get; set; }
        public int OverdueTasks    { get; set; }
        public List<ViewerProjectModel>  Projects    { get; set; } = new();
        public List<ViewerPostModel>     RecentPosts { get; set; } = new();
        public List<ViewerTaskListItemModel> RecentTasks { get; set; } = new();
    }

    public partial class ViewerProjectModel
    {
        public int     ID                { get; set; }
        public string  ProjectName       { get; set; } = null!;
        public string? Description       { get; set; }
        public int     TotalTasks        { get; set; }
        public int     CompletedTasks    { get; set; }
        public int     CompletionPercent { get; set; }
        public DateTime? StartDate       { get; set; }
        public DateTime? EndDate         { get; set; }
    }

    public class ViewerTaskListItemModel
    {
        public int      ID          { get; set; }
        public string   Title       { get; set; } = null!;
        public string?  Description { get; set; }
        public string?  ProjectName { get; set; }
        public string?  Status      { get; set; }
        public string   Priority    { get; set; } = "Medium";
        public DateTime? DueDate    { get; set; }
        public DateTime UpdatedAt   { get; set; }
    }

    public class ViewerTaskDetailsModel
    {
        public int      ID          { get; set; }
        public string   Title       { get; set; } = null!;
        public string?  Description { get; set; }
        public string?  ProjectName { get; set; }
        public string?  Status      { get; set; }
        public string   Priority    { get; set; } = "Medium";
        public DateTime? DueDate    { get; set; }
        public List<ViewerTaskStepModel>    Steps       { get; set; } = new();
        public List<ViewerCommentModel>     Comments    { get; set; } = new();
        public List<ViewerAttachmentModel>  Attachments { get; set; } = new();
    }

    public class ViewerTaskStepModel
    {
        public string?  Status          { get; set; }
        public string   PerformedByName { get; set; } = "Unknown";
        public DateTime DateTime        { get; set; }
    }

    public class ViewerCommentModel
    {
        public string  CommentText { get; set; } = null!;
        public string  UserName    { get; set; } = "Unknown";
        public DateTime CreatedAt  { get; set; }
    }

    public class ViewerAttachmentModel
    {
        public string   FileName       { get; set; } = null!;
        public string?  FilePath       { get; set; }
        public string   UploadedByName { get; set; } = "Unknown";
        public DateTime UpdatedAt      { get; set; }
    }

    public class ViewerPostModel
    {
        public int      ID        { get; set; }
        public string   Title     { get; set; } = null!;
        public string?  Content   { get; set; }
        public string   UserName  { get; set; } = "Unknown";
        public DateTime UpdatedAt { get; set; }
    }

    public class ViewerPostEditModel
    {
        public int      ID      { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string   Title   { get; set; } = null!;
        public string?  Content { get; set; }
    }

    public class ViewerReportModel
    {
        public int TotalProjects     { get; set; }
        public int TotalTasks        { get; set; }
        public int PendingTasks      { get; set; }
        public int InProgressTasks   { get; set; }
        public int CompletedTasks    { get; set; }
        public int CollaborationPosts{ get; set; }
        public List<TaskStatusCount>    TasksByStatus   { get; set; } = new();
        public List<TaskPriorityCount>  TasksByPriority { get; set; } = new();
        public List<ViewerProjectModel> ProjectProgress { get; set; } = new();
    }

    public class TaskStatusCount   { public string? Status   { get; set; } public int Count { get; set; } }
    public class TaskPriorityCount { public string  Priority { get; set; } = null!; public int Count { get; set; } }

    // ── Repo ─────────────────────────────────────────────────────────────────
    public class ViewerRepo(StmsDbContext context)
    {
        public Result<ViewerDashboardModel> GetDashboard()
        {
            var result = new Result<ViewerDashboardModel>();
            try
            {
                var tasks    = context.Tasks.Include(t => t.Project).Include(t => t.TaskSteps).ToList();
                var projects = context.Projects.Include(p => p.Tasks).ToList();
                var posts    = context.Posts.OrderByDescending(p => p.UpdatedAt).Take(5).ToList();
                var users    = context.Users.ToList();
                var now      = DateTime.Now;

                string GetStatus(TaskEntity t) =>
                    t.TaskSteps?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.Status ?? "Pending";

                var projectModels = projects.Select(p =>
                {
                    var ptasks    = p.Tasks.ToList();
                    var completed = ptasks.Count(t => GetStatus(t) == "Completed");
                    var pct       = ptasks.Count > 0 ? (int)Math.Round((double)completed / ptasks.Count * 100) : 0;
                    return new ViewerProjectModel
                    {
                        ID = p.ID, ProjectName = p.Title, Description = p.Description,
                        TotalTasks = ptasks.Count, CompletedTasks = completed, CompletionPercent = pct,
                        StartDate = p.CreatedAt, EndDate = null
                    };
                }).ToList();

                var postModels = posts.Select(p => new ViewerPostModel
                {
                    ID = p.ID, Title = p.Title, Content = p.Content, UpdatedAt = p.UpdatedAt,
                    UserName = users.FirstOrDefault(u => u.ID == p.UserID)?.Name ?? "Unknown"
                }).ToList();

                var recentTasks = tasks.OrderByDescending(t => t.UpdatedAt).Take(10).Select(t =>
                    new ViewerTaskListItemModel
                    {
                        ID = t.ID, Title = t.Title, Description = t.Description,
                        ProjectName = t.Project?.Title, Status = GetStatus(t),
                        Priority = t.Priority, DueDate = t.DueDate, UpdatedAt = t.UpdatedAt
                    }).ToList();

                result.Data = new ViewerDashboardModel
                {
                    TotalProjects   = projects.Count,
                    TotalTasks      = tasks.Count,
                    PendingTasks    = tasks.Count(t => GetStatus(t) == "Pending"),
                    InProgressTasks = tasks.Count(t => GetStatus(t) == "In Progress"),
                    CompletedTasks  = tasks.Count(t => GetStatus(t) == "Completed"),
                    OverdueTasks    = tasks.Count(t => t.DueDate.HasValue && t.DueDate < now && GetStatus(t) != "Completed"),
                    Projects    = projectModels,
                    RecentPosts = postModels,
                    RecentTasks = recentTasks
                };
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<List<ViewerTaskListItemModel>> GetAllTasks()
        {
            var result = new Result<List<ViewerTaskListItemModel>>();
            try
            {
                var tasks = context.Tasks.Include(t => t.Project).Include(t => t.TaskSteps).ToList();
                string GetStatus(TaskEntity t) =>
                    t.TaskSteps?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.Status ?? "Pending";

                result.Data = tasks.Select(t => new ViewerTaskListItemModel
                {
                    ID = t.ID, Title = t.Title, Description = t.Description,
                    ProjectName = t.Project?.Title, Status = GetStatus(t),
                    Priority = t.Priority, DueDate = t.DueDate, UpdatedAt = t.UpdatedAt
                }).ToList();
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<ViewerTaskDetailsModel> GetTaskDetails(int id)
        {
            var result = new Result<ViewerTaskDetailsModel>();
            try
            {
                var task = context.Tasks
                    .Include(t => t.Project)
                    .Include(t => t.TaskSteps)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .FirstOrDefault(t => t.ID == id);

                if (task == null) { result.HasError = true; result.Message = "Task Not Found"; return result; }

                var users = context.Users.ToList();
                string GetUserName(int uid) => users.FirstOrDefault(u => u.ID == uid)?.Name ?? "Unknown";

                result.Data = new ViewerTaskDetailsModel
                {
                    ID = task.ID, Title = task.Title, Description = task.Description,
                    ProjectName = task.Project?.Title, Priority = task.Priority, DueDate = task.DueDate,
                    Status = task.TaskSteps?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.Status ?? "Pending",
                    Steps = task.TaskSteps?.OrderByDescending(s => s.DateTime)
                        .Select(s => new ViewerTaskStepModel { Status = s.Status, DateTime = s.DateTime, PerformedByName = GetUserName(s.PerformedBy) })
                        .ToList() ?? new(),
                    Comments = task.Comments?.Select(c => new ViewerCommentModel
                        { CommentText = c.CommentText, CreatedAt = c.CreatedAt, UserName = GetUserName(c.UserID) })
                        .ToList() ?? new(),
                    Attachments = task.Attachments?.Select(a => new ViewerAttachmentModel
                        { FileName = a.FileName, FilePath = a.FilePath, UpdatedAt = a.UpdatedAt ?? DateTime.Now, UploadedByName = GetUserName(a.UploadedBy) })
                        .ToList() ?? new()
                };
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<List<ViewerPostModel>> GetPosts()
        {
            var result = new Result<List<ViewerPostModel>>();
            try
            {
                var posts = context.Posts.OrderByDescending(p => p.UpdatedAt).ToList();
                var users = context.Users.ToList();
                result.Data = posts.Select(p => new ViewerPostModel
                {
                    ID = p.ID, Title = p.Title, Content = p.Content, UpdatedAt = p.UpdatedAt,
                    UserName = users.FirstOrDefault(u => u.ID == p.UserID)?.Name ?? "Unknown"
                }).ToList();
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<Post> GetPostById(int id)
        {
            var result = new Result<Post>();
            try { result.Data = context.Posts.Find(id); }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<Post> SavePost(Post model, int userId)
        {
            var result = new Result<Post>();
            try
            {
                var objToSave = model.ID == 0 ? new Post() : (context.Posts.Find(model.ID) ?? new Post());
                if (model.ID == 0) { objToSave.CreatedAt = DateTime.Now; context.Posts.Add(objToSave); }
                objToSave.Title = model.Title; objToSave.Content = model.Content;
                objToSave.UserID = userId; objToSave.UpdatedAt = DateTime.Now;
                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<Post> DeletePost(int id)
        {
            var result = new Result<Post>();
            try
            {
                var obj = context.Posts.Find(id);
                if (obj == null) { result.HasError = true; result.Message = "Post Not Found"; return result; }
                context.Posts.Remove(obj);
                context.SaveChanges();
                result.Data = obj;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<ViewerReportModel> GetReport()
        {
            var result = new Result<ViewerReportModel>();
            try
            {
                var tasks    = context.Tasks.Include(t => t.Project).Include(t => t.TaskSteps).ToList();
                var projects = context.Projects.Include(p => p.Tasks).ToList();
                var posts    = context.Posts.ToList();

                string GetStatus(TaskEntity t) =>
                    t.TaskSteps?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.Status ?? "Pending";

                result.Data = new ViewerReportModel
                {
                    TotalProjects     = projects.Count,
                    TotalTasks        = tasks.Count,
                    PendingTasks      = tasks.Count(t => GetStatus(t) == "Pending"),
                    InProgressTasks   = tasks.Count(t => GetStatus(t) == "In Progress"),
                    CompletedTasks    = tasks.Count(t => GetStatus(t) == "Completed"),
                    CollaborationPosts= posts.Count,
                    TasksByStatus     = tasks.GroupBy(t => GetStatus(t)).Select(g => new TaskStatusCount { Status = g.Key, Count = g.Count() }).ToList(),
                    TasksByPriority   = tasks.GroupBy(t => t.Priority).Select(g => new TaskPriorityCount { Priority = g.Key, Count = g.Count() }).ToList(),
                    ProjectProgress   = projects.Select(p => {
                        var pt = p.Tasks.ToList();
                        var comp = pt.Count(t => GetStatus(t) == "Completed");
                        var pct = pt.Count > 0 ? (int)Math.Round((double)comp / pt.Count * 100) : 0;
                        return new ViewerProjectModel {
                            ID = p.ID, ProjectName = p.Title,
                            TotalTasks = pt.Count, CompletedTasks = comp, CompletionPercent = pct,
                            PendingTasks = pt.Count(t => GetStatus(t) == "Pending"),
                            InProgressTasks = pt.Count(t => GetStatus(t) == "In Progress")
                        };
                    }).ToList()
                };
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }
    }

    // Extra props for project model used in reports
    public partial class ViewerProjectModel
    {
        public int PendingTasks    { get; set; }
        public int InProgressTasks { get; set; }
    }
}
