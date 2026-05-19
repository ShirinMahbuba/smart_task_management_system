using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    public class CommentController(CommentRepo commentRepo) : Controller
    {
        public IActionResult Index(int taskId)
        {
            var result = commentRepo.GetByTaskId(taskId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<Comment>());
            }
            ViewBag.TaskId = taskId;
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Detail(int dataId, int taskId)
        {
            if (dataId == -1)
            {
                return View(new Comment { TaskID = taskId, UserID = CurrentUserHelper.GetUserId(User) });
            }

            var result = commentRepo.GetById(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", new { taskId });
            }
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Detail(Comment model)
        {
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UserID");

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            model.UserID = CurrentUserHelper.GetUserId(User);
            model.CreatedAt = DateTime.Now;

            var result = commentRepo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            else
            {
                if (result.Data != null)
                {
                    TempData["Success"] = $"Comment#{result.Data.ID} saved Successfully";
                }
                return RedirectToAction("Index", new { taskId = model.TaskID });
            }
        }

        public IActionResult Delete(int dataId, int taskId)
        {
            var result = commentRepo.Delete(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = $"Comment#{dataId} deleted Successfully";
            }
            return RedirectToAction("Index", new { taskId });
        }
    }
}