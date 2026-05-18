using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Models;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    [Authorize(Roles = "Viewer")]
    public class ViewerController(ViewerRepo viewerRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = viewerRepo.GetDashboardData();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new ViewerDashboardModel());
            }

            return View(result.Data);
        }

        public IActionResult Tasks()
        {
            var result = viewerRepo.GetTasks();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<ViewerTaskListItemModel>());
            }

            return View(result.Data);
        }

        public IActionResult TaskDetails(int id)
        {
            var result = viewerRepo.GetTaskDetails(id);
            if (result.HasError || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Tasks");
            }

            return View(result.Data);
        }

        public IActionResult Posts()
        {
            var result = viewerRepo.GetPosts();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<ViewerPostModel>());
            }

            return View(result.Data);
        }

        public IActionResult Discussion()
        {
            return RedirectToAction("Posts");
        }

        public IActionResult PostForm(int dataId = 0)
        {
            if (dataId == 0)
                return View(new ViewerPostEditModel());

            var result = viewerRepo.GetPostForEdit(dataId);
            if (result.HasError || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Posts");
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostForm(ViewerPostEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = viewerRepo.SavePost(model, CurrentUserHelper.GetUserId(User));
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Collaboration post saved successfully";
            return RedirectToAction("Posts");
        }

        public IActionResult DeletePost(int dataId)
        {
            var result = viewerRepo.DeletePost(dataId, CurrentUserHelper.GetUserId(User));
            TempData[result.HasError ? "Error" : "Success"] = result.HasError
                ? result.Message
                : "Collaboration post deleted successfully";

            return RedirectToAction("Posts");
        }

        public IActionResult Reports()
        {
            var result = viewerRepo.GetReports();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new ViewerReportModel());
            }

            return View(result.Data);
        }
    }
}
