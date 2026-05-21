using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;
using Task = STMS.Entities.Task;

namespace STMS.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TaskManagerController : Controller
    {
        private readonly TaskRepo taskRepo;
        private readonly UserRepo userRepo;

        public TaskManagerController(TaskRepo taskRepo, UserRepo userRepo)
        {
            this.taskRepo = taskRepo;
            this.userRepo = userRepo;
        }

        public IActionResult EditTask(int taskID)
        {
            var result = taskRepo.GetByIdForEdit(taskID);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Project");
            }
            var data = userRepo.GetAll();
            var employees = data.Data?.Where(u => u.Role == "Employee").ToList() ?? new List<User>();
            ViewBag.Employees = employees;
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult EditTask(Task model, int? NewAssigneeID)
        {
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("Project");
            ModelState.Remove("TaskAssignments");
            ModelState.Remove("TaskSteps");

            if (!ModelState.IsValid)
            {
                var data = userRepo.GetAll();
                var employees = data.Data?.Where(u => u.Role == "Employee").ToList() ?? new List<User>();
                ViewBag.Employees = employees;
                return View(model);
            }

            if (NewAssigneeID.HasValue)
            {
                if (model.TaskAssignments == null)
                    model.TaskAssignments = new List<TaskAssignment>();

                if (!model.TaskAssignments.Any(a => a.UserID == NewAssigneeID.Value))
                {
                    model.TaskAssignments.Add(new TaskAssignment
                    {
                        UserID = NewAssigneeID.Value,
                        TaskID = model.ID
                    });
                }
            }

            model.CreatedBy = int.Parse(User.FindFirst("UserId").Value);
            var result = taskRepo.Update(model);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Project");
            }

            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction("EditTask", new { taskID = model.ID });
        }

        public IActionResult DeleteTaskAssignment(int taskID, int assignmentID)
        {
            var result = taskRepo.GetByIdForEdit(taskID);
            if (result.HasError || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Task not found.";
                return RedirectToAction("Index", "Project");
            }

            var task = result.Data;
            var assignment = task.TaskAssignments?.FirstOrDefault(a => a.ID == assignmentID);
            if (assignment == null)
            {
                TempData["Error"] = "Assignment not found.";
                return RedirectToAction("EditTask", new { taskID });
            }

            try
            {
                task.TaskAssignments.Remove(assignment);
                taskRepo.Update(task);
                TempData["Success"] = "Assignment removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error removing assignment: " + ex.Message;
            }

            return RedirectToAction("EditTask", new { taskID });
        }
    }
}
