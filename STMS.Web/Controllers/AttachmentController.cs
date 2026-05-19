using Microsoft.AspNetCore.Mvc;
using STMS.Repos;
using STMS.Shared;
using AttachmentEntity = STMS.Entities.Attachment;

namespace STMS.Web.Controllers
{
    public class AttachmentController(AttachmentRepo attachmentRepo) : Controller
    {
        public IActionResult Index(int taskId)
        {
            var result = attachmentRepo.GetByTaskId(taskId);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(new List<AttachmentEntity>());
            }
            ViewBag.TaskId = taskId;
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Upload(int taskId)
        {
            return View(new AttachmentEntity { TaskID = taskId });
        }

        [HttpPost]
        public IActionResult Upload(AttachmentEntity model, IFormFile file)
        {
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("FileName");
            ModelState.Remove("FilePath");

            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a file";
                return View(model);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            int currentUserId = CurrentUserHelper.GetUserId(User);
            model.FileName = fileName;
            model.FilePath = "/uploads/" + fileName;
            model.UpdatedAt = DateTime.Now;
            model.UpdatedBy = currentUserId;
            model.UploadedBy = currentUserId;

            var result = attachmentRepo.Save(model);
            if (result.HasError)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }
            else
            {
                if (result.Data != null)
                {
                    TempData["Success"] = $"File uploaded Successfully";
                }
                return RedirectToAction("Index", new { taskId = model.TaskID });
            }
        }

        public IActionResult Delete(int dataId, int taskId)
        {
            var result = attachmentRepo.Delete(dataId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = $"Attachment#{dataId} deleted Successfully";
            }
            return RedirectToAction("Index", new { taskId });
        }
    }
}