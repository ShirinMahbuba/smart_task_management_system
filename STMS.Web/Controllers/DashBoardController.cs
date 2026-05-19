using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    [Authorize]
    public class DashboardController(TaskRepo taskRepo) : Controller
    {
        public IActionResult Index()
        {
            int employeeId = CurrentUserHelper.GetUserId(User);

            var result = taskRepo.GetAssignedTasks(employeeId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View();
            }

            ViewBag.TotalTasks = result.Data?.Count ?? 0;
            ViewBag.PendingTasks = result.Data?
                .Count(t => t.TaskSteps?
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefault()?.Status == "Pending") ?? 0;
            ViewBag.InProgressTasks = result.Data?
                .Count(t => t.TaskSteps?
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefault()?.Status == "In Progress") ?? 0;
            ViewBag.CompletedTasks = result.Data?
                .Count(t => t.TaskSteps?
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefault()?.Status == "Completed") ?? 0;

            return View();
        }
    }
}