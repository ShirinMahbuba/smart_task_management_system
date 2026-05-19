using STMS.Data;
using STMS.Entities;
using STMS.Shared;
using Microsoft.EntityFrameworkCore;
using TaskEntity = STMS.Entities.Task;

namespace STMS.Repos
{
    public class TaskRepo(StmsDbContext context)
    {
        public Result<List<TaskEntity>> GetAssignedTasks(int employeeId)
        {
            var result = new Result<List<TaskEntity>>();
            try
            {
                result.Data = context.Tasks
                    .Include(t => t.TaskAssignments)
                    .Include(t => t.TaskSteps)
                    .Where(t => t.TaskAssignments!
                        .Any(a => a.UserID == employeeId)
                        && t.Deleted == false)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<TaskEntity> GetById(int id)
        {
            var result = new Result<TaskEntity>();
            try
            {
                result.Data = context.Tasks
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Include(t => t.TaskSteps)
                    .FirstOrDefault(t => t.ID == id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }
    }
}