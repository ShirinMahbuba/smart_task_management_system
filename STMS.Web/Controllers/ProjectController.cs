using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;

namespace STMS.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ProjectController(ProjectRepo projectRepo) : Controller
    {
        public IActionResult Index()
        {
            var result = projectRepo.GetAll();
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }

        public IActionResult Detail(int projectID)
        {
            if (projectID == -1)
            {
                return View(new Project());
            }

            var result = projectRepo.GetById(projectID);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult Detail(Project model)
        {
            if (model.ID == 0)
            {
                ModelState.Remove("ID");
            }
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("Creator");
            ModelState.Remove("Tasks");

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (model.EndDate < model.StartDate)
            {
                ViewBag.Error = "End Date cannot be earlier than Start Date.";
                return View(model);
            }

            if (model.StartDate < DateTime.Today.AddDays(-30))
            {
                ViewBag.Error = "Start Date cannot be more than 30 days in the past.";
                return View(model);
            }

            var existing = projectRepo.GetAll().Data?.FirstOrDefault(p => p.ProjectName == model.ProjectName && p.ID != model.ID);
            if (existing != null)
            {
                ViewBag.Error = "A project with the same name already exists.";
                return View(model);
            }

            var result = projectRepo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            TempData["Success"] = "Project saved successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int projectID)
        {
            var result = projectRepo.Delete(projectID);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "Event pricing deleted successfully.";
            }
            return RedirectToAction("Index");
        }

    }
}
