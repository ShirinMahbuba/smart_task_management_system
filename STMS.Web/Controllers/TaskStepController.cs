using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    public class TaskStepController(TaskStepRepo taskStepRepo) : Controller
    {
        [HttpGet]
        public IActionResult Update(int taskId)
        {
            return View(new TaskStep { TaskID = taskId });
        }

        [HttpPost]
        public IActionResult Update(TaskStep model)
        {
            ModelState.Remove("StepDate");

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            model.PerformedBy = CurrentUserHelper.GetUserId(User);

            var result = taskStepRepo.UpdateStatus(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            else
            {
                if (result.Data != null)
                {
                    TempData["Success"] = $"Task#{result.Data.TaskID} status updated Successfully";
                }
                return RedirectToAction("Index", "Task");
            }
        }
    }
}