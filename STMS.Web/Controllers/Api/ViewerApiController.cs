using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Models;
using STMS.Repos;
using STMS.Shared;

namespace STMS.Web.Controllers.Api
{
    [ApiController]
    [Route("api/viewer")]
    [Authorize(Roles = "Viewer")]
    public class ViewerApiController(ViewerRepo viewerRepo) : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var result = viewerRepo.GetDashboardData();
            return ToActionResult(result);
        }

        [HttpGet("projects")]
        public IActionResult Projects()
        {
            var result = viewerRepo.GetProjects();
            return ToActionResult(result);
        }

        [HttpGet("tasks")]
        public IActionResult Tasks()
        {
            var result = viewerRepo.GetTasks();
            return ToActionResult(result);
        }

        [HttpGet("tasks/{id:int}")]
        public IActionResult TaskDetails(int id)
        {
            var result = viewerRepo.GetTaskDetails(id);
            return ToActionResult(result);
        }

        [HttpGet("posts")]
        public IActionResult Posts()
        {
            var result = viewerRepo.GetPosts();
            return ToActionResult(result);
        }

        [HttpPost("posts")]
        public IActionResult CreatePost(ViewerPostEditModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = viewerRepo.SavePost(model, CurrentUserHelper.GetUserId(User));
            return ToActionResult(result);
        }

        [HttpPut("posts/{id:int}")]
        public IActionResult UpdatePost(int id, ViewerPostEditModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            model.ID = id;
            var result = viewerRepo.SavePost(model, CurrentUserHelper.GetUserId(User));
            return ToActionResult(result);
        }

        [HttpDelete("posts/{id:int}")]
        public IActionResult DeletePost(int id)
        {
            var result = viewerRepo.DeletePost(id, CurrentUserHelper.GetUserId(User));
            return ToActionResult(result);
        }

        [HttpGet("reports")]
        public IActionResult Reports()
        {
            var result = viewerRepo.GetReports();
            return ToActionResult(result);
        }

        private IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}
