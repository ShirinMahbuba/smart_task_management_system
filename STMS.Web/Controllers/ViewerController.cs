using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    [Authorize(Roles = "Viewer")]
    public class ViewerController(ViewerRepo viewerRepo) : Controller
    {
        // GET: /Viewer/Index — Project Dashboard
        public IActionResult Index()
        {
            var result = viewerRepo.GetDashboard();
            if (result.HasError) { ViewBag.Error = result.Message; return View(new ViewerDashboardModel()); }
            return View(result.Data);
        }

        // GET: /Viewer/Tasks
        public IActionResult Tasks()
        {
            var result = viewerRepo.GetAllTasks();
            if (result.HasError) { ViewBag.Error = result.Message; return View(new List<ViewerTaskListItemModel>()); }
            return View(result.Data);
        }

        // GET: /Viewer/TaskDetails?id=5
        public IActionResult TaskDetails(int id)
        {
            var result = viewerRepo.GetTaskDetails(id);
            if (result.HasError) { TempData["Error"] = result.Message; return RedirectToAction("Tasks"); }
            return View(result.Data);
        }

        // GET: /Viewer/Posts
        public IActionResult Posts()
        {
            var result = viewerRepo.GetPosts();
            if (result.HasError) { ViewBag.Error = result.Message; return View(new List<ViewerPostModel>()); }
            return View(result.Data);
        }

        // GET: /Viewer/PostForm?dataId=-1 or dataId=5
        public IActionResult PostForm(int dataId = -1)
        {
            if (dataId == -1)
                return View(new ViewerPostEditModel());

            var result = viewerRepo.GetPostById(dataId);
            if (result.HasError) { TempData["Error"] = result.Message; return RedirectToAction("Posts"); }

            return View(new ViewerPostEditModel
            {
                ID      = result.Data!.ID,
                Title   = result.Data.Title,
                Content = result.Data.Content
            });
        }

        // POST: /Viewer/PostForm
        [HttpPost]
        public IActionResult PostForm(ViewerPostEditModel model)
        {
            if (ModelState.IsValid == false) return View(model);

            var post = new Post { ID = model.ID, Title = model.Title, Content = model.Content };
            var result = viewerRepo.SavePost(post, CurrentUserHelper.GetUserId(User));

            if (result.HasError) { ViewBag.Error = result.Message; return View(model); }
            TempData["Success"] = "Post saved successfully";
            return RedirectToAction("Posts");
        }

        // GET: /Viewer/DeletePost?dataId=5
        public IActionResult DeletePost(int dataId)
        {
            var result = viewerRepo.DeletePost(dataId);
            if (result.HasError) TempData["Error"] = result.Message;
            else TempData["Success"] = $"Post#{dataId} deleted";
            return RedirectToAction("Posts");
        }

        // GET: /Viewer/Reports
        public IActionResult Reports()
        {
            var result = viewerRepo.GetReport();
            if (result.HasError) { ViewBag.Error = result.Message; return View(new ViewerReportModel()); }
            return View(result.Data);
        }
    }
}
