using Microsoft.EntityFrameworkCore;
using STMS.Data;
using STMS.Entities;
using STMS.Models;
using STMS.Shared;
using Task = STMS.Entities.Task;

namespace STMS.Repos
{
    public class ViewerRepo(StmsDbContext context)
    {
        public Result<ViewerDashboardModel> GetDashboardData()
        {
            var result = new Result<ViewerDashboardModel>();
            try
            {
                var tasks = GetActiveTasksQuery()
                    .Include(t => t.Project)
                    .AsNoTracking()
                    .ToList();

                var projects = context.Projects
                    .Include(p => p.Tasks.Where(t => !t.Deleted))
                    .AsNoTracking()
                    .OrderBy(p => p.ProjectName)
                    .ToList();

                result.Data = new ViewerDashboardModel
                {
                    TotalProjects = projects.Count,
                    TotalTasks = tasks.Count,
                    PendingTasks = tasks.Count(t => IsStatus(t.Status, "Pending")),
                    InProgressTasks = tasks.Count(t => IsStatus(t.Status, "In Progress")),
                    CompletedTasks = tasks.Count(t => IsStatus(t.Status, "Completed")),
                    OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.Today && !IsStatus(t.Status, "Completed")),
                    HighPriorityTasks = tasks.Count(t => IsStatus(t.Priority, "High")),
                    Projects = projects.Select(MapProjectSummary).ToList(),
                    RecentTasks = tasks
                        .OrderByDescending(t => t.UpdatedAt)
                        .ThenBy(t => t.DueDate)
                        .Take(8)
                        .Select(MapTaskListItem)
                        .ToList(),
                    RecentPosts = GetPostsInternal().Take(5).ToList()
                };
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<ViewerProjectSummaryModel>> GetProjects()
        {
            var result = new Result<List<ViewerProjectSummaryModel>>();
            try
            {
                result.Data = context.Projects
                    .Include(p => p.Tasks.Where(t => !t.Deleted))
                    .AsNoTracking()
                    .OrderBy(p => p.ProjectName)
                    .ToList()
                    .Select(MapProjectSummary)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<ViewerTaskListItemModel>> GetTasks()
        {
            var result = new Result<List<ViewerTaskListItemModel>>();
            try
            {
                result.Data = GetActiveTasksQuery()
                    .Include(t => t.Project)
                    .AsNoTracking()
                    .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                    .ThenBy(t => t.Title)
                    .ToList()
                    .Select(MapTaskListItem)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<ViewerTaskDetailsModel> GetTaskDetails(int taskId)
        {
            var result = new Result<ViewerTaskDetailsModel>();
            try
            {
                var task = GetActiveTasksQuery()
                    .Include(t => t.Project)
                    .Include(t => t.TaskSteps)
                    .Include(t => t.Comments).ThenInclude(c => c.User)
                    .Include(t => t.Attachments)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.ID == taskId);

                if (task == null)
                {
                    result.HasError = true;
                    result.Message = "Task not found";
                    return result;
                }

                var userNames = context.Users.AsNoTracking().ToDictionary(u => u.ID, u => u.Name);
                var model = new ViewerTaskDetailsModel
                {
                    ID = task.ID,
                    ProjectID = task.ProjectID,
                    Title = task.Title,
                    Description = task.Description,
                    ProjectName = task.Project?.ProjectName ?? string.Empty,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    UpdatedAt = task.UpdatedAt,
                    Steps = task.TaskSteps
                        .OrderByDescending(s => s.DateTime)
                        .Select(s => new ViewerTaskStepModel
                        {
                            ID = s.ID,
                            Status = s.Status,
                            DateTime = s.DateTime,
                            PerformedByName = userNames.GetValueOrDefault(s.PerformedBy, "Unknown")
                        })
                        .ToList(),
                    Comments = task.Comments
                        .OrderByDescending(c => c.CreatedAt)
                        .Select(c => new ViewerCommentModel
                        {
                            ID = c.ID,
                            CommentText = c.CommentText,
                            UserName = c.User?.Name ?? "Unknown",
                            CreatedAt = c.CreatedAt
                        })
                        .ToList(),
                    Attachments = task.Attachments
                        .Where(a => !a.Deleted)
                        .OrderByDescending(a => a.UpdatedAt)
                        .Select(a => new ViewerAttachmentModel
                        {
                            ID = a.ID,
                            FileName = a.FileName,
                            FilePath = a.FilePath,
                            UpdatedAt = a.UpdatedAt,
                            UploadedByName = userNames.GetValueOrDefault(a.UploadedBy, "Unknown")
                        })
                        .ToList()
                };

                result.Data = model;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<List<ViewerPostModel>> GetPosts()
        {
            var result = new Result<List<ViewerPostModel>>();
            try
            {
                result.Data = GetPostsInternal().ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<ViewerPostEditModel> GetPostForEdit(int id)
        {
            var result = new Result<ViewerPostEditModel>();
            try
            {
                var post = context.Posts.AsNoTracking().FirstOrDefault(p => p.ID == id);
                if (post == null)
                {
                    result.HasError = true;
                    result.Message = "Post not found";
                    return result;
                }

                result.Data = new ViewerPostEditModel
                {
                    ID = post.ID,
                    Title = post.Title,
                    Content = post.Content ?? string.Empty
                };
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Post> SavePost(ViewerPostEditModel model, int userId)
        {
            var result = new Result<Post>();
            try
            {
                var post = model.ID == 0 ? new Post() : context.Posts.FirstOrDefault(p => p.ID == model.ID);
                if (post == null)
                {
                    result.HasError = true;
                    result.Message = "Post not found";
                    return result;
                }

                if (model.ID == 0)
                {
                    post.UserID = userId;
                    post.CreatedAt = DateTime.Now;
                    context.Posts.Add(post);
                }

                post.Title = model.Title.Trim();
                post.Content = model.Content.Trim();
                post.UpdatedAt = DateTime.Now;

                context.SaveChanges();
                context.ActivityLogs.Add(new ActivityLog
                {
                    UserID = userId,
                    Action = $"Saved viewer collaboration post #{post.ID}",
                    LogTime = DateTime.Now
                });
                context.SaveChanges();
                result.Data = post;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Post> DeletePost(int id, int userId)
        {
            var result = new Result<Post>();
            try
            {
                var post = context.Posts.FirstOrDefault(p => p.ID == id);
                if (post == null)
                {
                    result.HasError = true;
                    result.Message = "Post not found";
                    return result;
                }

                context.Posts.Remove(post);
                context.ActivityLogs.Add(new ActivityLog
                {
                    UserID = userId,
                    Action = $"Deleted viewer collaboration post #{id}",
                    LogTime = DateTime.Now
                });
                context.SaveChanges();
                result.Data = post;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<ViewerReportModel> GetReports()
        {
            var result = new Result<ViewerReportModel>();
            try
            {
                var tasks = GetActiveTasksQuery()
                    .Include(t => t.Project)
                    .AsNoTracking()
                    .ToList();

                var projects = context.Projects
                    .Include(p => p.Tasks.Where(t => !t.Deleted))
                    .AsNoTracking()
                    .OrderBy(p => p.ProjectName)
                    .ToList();

                result.Data = new ViewerReportModel
                {
                    TotalProjects = projects.Count,
                    TotalTasks = tasks.Count,
                    PendingTasks = tasks.Count(t => IsStatus(t.Status, "Pending")),
                    InProgressTasks = tasks.Count(t => IsStatus(t.Status, "In Progress")),
                    CompletedTasks = tasks.Count(t => IsStatus(t.Status, "Completed")),
                    OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.Today && !IsStatus(t.Status, "Completed")),
                    CollaborationPosts = context.Posts.Count(),
                    TasksByStatus = tasks
                        .GroupBy(t => t.Status)
                        .Select(g => new ViewerStatusCountModel { Status = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Status)
                        .ToList(),
                    TasksByPriority = tasks
                        .GroupBy(t => t.Priority)
                        .Select(g => new ViewerPriorityCountModel { Priority = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Priority)
                        .ToList(),
                    ProjectProgress = projects.Select(MapProjectSummary).ToList()
                };
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        private IQueryable<Task> GetActiveTasksQuery()
        {
            return context.Tasks.Where(t => !t.Deleted);
        }

        private List<ViewerPostModel> GetPostsInternal()
        {
            return context.Posts
                .Include(p => p.User)
                .AsNoTracking()
                .OrderByDescending(p => p.UpdatedAt)
                .ThenByDescending(p => p.CreatedAt)
                .Select(p => new ViewerPostModel
                {
                    ID = p.ID,
                    UserID = p.UserID,
                    UserName = p.User != null ? p.User.Name : "Unknown",
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();
        }

        private static ViewerTaskListItemModel MapTaskListItem(Task task)
        {
            return new ViewerTaskListItemModel
            {
                ID = task.ID,
                ProjectID = task.ProjectID,
                Title = task.Title,
                Description = task.Description,
                ProjectName = task.Project?.ProjectName ?? string.Empty,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                UpdatedAt = task.UpdatedAt
            };
        }

        private static ViewerProjectSummaryModel MapProjectSummary(Project project)
        {
            var tasks = project.Tasks.Where(t => !t.Deleted).ToList();
            var total = tasks.Count;
            var completed = tasks.Count(t => IsStatus(t.Status, "Completed"));

            return new ViewerProjectSummaryModel
            {
                ID = project.ID,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                TotalTasks = total,
                CompletedTasks = completed,
                InProgressTasks = tasks.Count(t => IsStatus(t.Status, "In Progress")),
                PendingTasks = tasks.Count(t => IsStatus(t.Status, "Pending")),
                CompletionPercent = total == 0 ? 0 : Math.Round((decimal)completed * 100 / total, 2)
            };
        }

        private static bool IsStatus(string? value, string expected)
        {
            return string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
