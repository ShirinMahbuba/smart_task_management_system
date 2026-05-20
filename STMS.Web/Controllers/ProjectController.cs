using Microsoft.AspNetCore.Mvc;
using STMS.Repos;

namespace STMS.Web.Controllers
{
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
    }
}
