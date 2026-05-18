using STMS.Data;
using STMS.Entities;
using STMS.Models;
using STMS.Shared;

namespace STMS.Repos
{
    public class UserRepo(StmsDbContext context)
    {
        public Result<User> Login(string email, string password)
        {
            var result = new Result<User>();
            try
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.Email == email && u.Password == password && u.IsActive == true);
                if (user != null)
                {
                    result.Data = user;
                    context.ActivityLogs.Add(new ActivityLog { UserID = user.ID, Action = $"User logged in ({user.Email})", LogTime = DateTime.Now });
                    context.SaveChanges();
                }
                else { result.HasError = true; result.Message = "Invalid Email or Password"; }
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<User> Register(RegisterModel model)
        {
            var result = new Result<User>();
            try
            {
                if (context.Users.Any(u => u.Email == model.Email))
                { result.HasError = true; result.Message = "Email Already Exists"; return result; }

                var newUser = new User { Name = model.Name, Email = model.Email, Password = model.Password, Role = "Viewer", IsActive = true, UpdatedAt = DateTime.Now, UpdatedBy = 0 };
                context.Users.Add(newUser);
                context.SaveChanges();
                context.ActivityLogs.Add(new ActivityLog { UserID = newUser.ID, Action = $"New account registered ({newUser.Email})", LogTime = DateTime.Now });
                context.SaveChanges();
                result.Data = newUser;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<User> ResetPassword(PasswordResetModel model)
        {
            var result = new Result<User>();
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null) { result.HasError = true; result.Message = "No account found with this email"; return result; }
                user.Password = model.NewPassword; user.UpdatedAt = DateTime.Now;
                context.SaveChanges();
                context.ActivityLogs.Add(new ActivityLog { UserID = user.ID, Action = $"Password reset ({user.Email})", LogTime = DateTime.Now });
                context.SaveChanges();
                result.Data = user;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<List<User>> GetAll()
        {
            var result = new Result<List<User>>();
            try { result.Data = context.Users.ToList(); }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<User> GetById(int id)
        {
            var result = new Result<User>();
            try { result.Data = context.Users.Find(id); }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<User> Save(User model, int loggedInUserId)
        {
            var result = new Result<User>();
            try
            {
                if (context.Users.Any(u => u.Email == model.Email && u.ID != model.ID))
                { result.HasError = true; result.Message = "Email Already Exists"; return result; }

                var objToSave = context.Users.Find(model.ID);
                if (objToSave == null) { objToSave = new User(); context.Users.Add(objToSave); }
                objToSave.Name = model.Name; objToSave.Email = model.Email;
                if (!string.IsNullOrWhiteSpace(model.Password)) objToSave.Password = model.Password;
                objToSave.Role = model.Role; objToSave.IsActive = model.IsActive;
                objToSave.UpdatedAt = DateTime.Now; objToSave.UpdatedBy = loggedInUserId;
                context.SaveChanges();
                context.ActivityLogs.Add(new ActivityLog { UserID = loggedInUserId, Action = $"Saved User#{objToSave.ID} ({objToSave.Name})", LogTime = DateTime.Now });
                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<User> Delete(int id, int loggedInUserId)
        {
            var result = new Result<User>();
            try
            {
                var objToDelete = context.Users.Find(id);
                if (objToDelete == null) { result.HasError = true; result.Message = "User Not Found"; return result; }
                context.ActivityLogs.Add(new ActivityLog { UserID = loggedInUserId, Action = $"Deleted User#{id} ({objToDelete.Name})", LogTime = DateTime.Now });
                context.Users.Remove(objToDelete);
                context.SaveChanges();
                result.Data = objToDelete;
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<List<ActivityLog>> GetActivityLogs()
        {
            var result = new Result<List<ActivityLog>>();
            try { result.Data = context.ActivityLogs.OrderByDescending(a => a.LogTime).ToList(); }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<AdminDashboardData> GetDashboardData()
        {
            var result = new Result<AdminDashboardData>();
            try
            {
                var users = context.Users.ToList();
                var logs  = context.ActivityLogs.OrderByDescending(l => l.LogTime).Take(5).ToList();

                // Safe — try individually so Status column error doesn't crash dashboard
                int totalProjects = 0, totalTasks = 0, pendingTasks = 0, completedTasks = 0;
                try { totalProjects = context.Projects.Count(); } catch { }
                try
                {
                    var tasks = context.Tasks.ToList();
                    totalTasks     = tasks.Count;
                    pendingTasks   = tasks.Count(t => t.Status == "Pending");
                    completedTasks = tasks.Count(t => t.Status == "Completed");
                }
                catch { }

                result.Data = new AdminDashboardData
                {
                    TotalUsers     = users.Count,
                    ActiveUsers    = users.Count(u => u.IsActive),
                    InactiveUsers  = users.Count(u => !u.IsActive),
                    TotalProjects  = totalProjects,
                    TotalTasks     = totalTasks,
                    PendingTasks   = pendingTasks,
                    CompletedTasks = completedTasks,
                    ByRole         = users.GroupBy(u => u.Role).Select(g => new RoleCount { Role = g.Key, Count = g.Count() }).ToList(),
                    RecentLogs     = logs
                };
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }
    }

    public class AdminDashboardData
    {
        public int TotalUsers     { get; set; }
        public int ActiveUsers    { get; set; }
        public int InactiveUsers  { get; set; }
        public int TotalProjects  { get; set; }
        public int TotalTasks     { get; set; }
        public int PendingTasks   { get; set; }
        public int CompletedTasks { get; set; }
        public List<RoleCount>   ByRole     { get; set; } = new();
        public List<ActivityLog> RecentLogs { get; set; } = new();
    }

    public class RoleCount
    {
        public string Role  { get; set; } = null!;
        public int    Count { get; set; }
    }
}
