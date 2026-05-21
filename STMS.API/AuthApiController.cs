using Microsoft.AspNetCore.Mvc;
using STMS.Repos;
using STMS.Models;

namespace STMS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController(UserRepo userRepo) : ControllerBase
    {
        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = userRepo.Login(model.Email, model.Password);

            // ????? ?????? 401 ????????? ??? ??????? ??? ?????, ?? ???? ??? ???? ???? ??
            if (result.HasError || result.Data == null)
            {
                return StatusCode(401, new { message = result.Message });
            }

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    id = result.Data.ID,
                    name = result.Data.Name,
                    email = result.Data.Email,
                    role = result.Data.Role
                }
            });
        }
    }
}