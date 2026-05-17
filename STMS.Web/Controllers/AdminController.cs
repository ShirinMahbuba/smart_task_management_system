using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(UserRepo userRepo) : Controller
    {
        // ── DASHBOARD ─────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            var result = userRepo.GetDashboardData();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new AdminDashboardData());
            }
            return View(result.Data);
        }

        // ── USER LIST ──────────────────────────────────────────────────────────
        public IActionResult Users()
        {
            var result = userRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<User>());
            }
            return View(result.Data);
        }

        // ── DETAIL GET ─────────────────────────────────────────────────────────
        public IActionResult Detail(int dataId)
        {
            if (dataId == -1)
                return View(new User());

            var result = userRepo.GetById(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Users");
            }
            return View(result.Data);
        }

        // ── DETAIL POST ────────────────────────────────────────────────────────
        [HttpPost]
        public IActionResult Detail(User model)
        {
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("UpdatedBy");
            if (model.ID != 0)
                ModelState.Remove("Password");

            if (ModelState.IsValid == false)
                return View(model);

            var result = userRepo.Save(model, CurrentUserHelper.GetUserId(User));

            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            else
            {
                if (result.Data != null)
                    TempData["Success"] = $"Data#{result.Data.ID} saved Successfully";
                return RedirectToAction("Users");
            }
        }

        // ── DELETE ─────────────────────────────────────────────────────────────
        public IActionResult Delete(int dataId)
        {
            var result = userRepo.Delete(dataId, CurrentUserHelper.GetUserId(User));
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = $"Data#{dataId} deleted Successfully";

            return RedirectToAction("Users");
        }

        // ── ACTIVITY LOGS ──────────────────────────────────────────────────────
        public IActionResult ActivityLogs()
        {
            var result = userRepo.GetActivityLogs();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<ActivityLog>());
            }
            return View(result.Data);
        }

        // ── REPORTS ────────────────────────────────────────────────────────────
        public IActionResult Reports()
        {
            var dashResult  = userRepo.GetDashboardData();
            var usersResult = userRepo.GetAll();
            var logsResult  = userRepo.GetActivityLogs();

            ViewBag.Dashboard  = dashResult.Data;
            ViewBag.AllUsers   = usersResult.Data ?? new List<User>();
            ViewBag.AllLogs    = logsResult.Data?.Take(20).ToList() ?? new List<ActivityLog>();
            ViewBag.LogsThisMonth = logsResult.Data?.Count(l =>
                l.LogTime.Month == DateTime.Now.Month &&
                l.LogTime.Year  == DateTime.Now.Year) ?? 0;

            return View();
        }
    }
}
