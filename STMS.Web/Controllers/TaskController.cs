using Microsoft.AspNetCore.Mvc;
using STMS.Repos;
using STMS.Shared;
using TaskEntity = STMS.Entities.Task;

namespace STMS.Web.Controllers
{
    public class TaskController(TaskRepo taskRepo) : Controller
    {
        public IActionResult Index()
        {
            int employeeId = CurrentUserHelper.GetUserId(User);

            var result = taskRepo.GetAssignedTasks(employeeId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<TaskEntity>());
            }
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Detail(int dataId)
        {
            var result = taskRepo.GetById(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }
    }
}