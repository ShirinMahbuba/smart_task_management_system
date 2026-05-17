using Microsoft.EntityFrameworkCore;
using STMS.Entities;
using Task = STMS.Entities.Task;

namespace STMS.Data
{
    public class StmsDbContext(DbContextOptions<StmsDbContext> options) : DbContext(options)
    {
        public DbSet<User>           Users           { get; set; }
        public DbSet<ActivityLog>    ActivityLogs    { get; set; }
        public DbSet<Project>        Projects        { get; set; }
        public DbSet<Task>           Tasks           { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskStep>       TaskSteps       { get; set; }
        public DbSet<TaskDependency> TaskDependencies{ get; set; }
        public DbSet<Post>           Posts           { get; set; }
        public DbSet<Comment>        Comments        { get; set; }
        public DbSet<Attachment>     Attachments     { get; set; }
    }
}
