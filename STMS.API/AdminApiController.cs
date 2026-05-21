using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STMS.Entities;
using STMS.Repos;

namespace STMS.Web.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminApiController(UserRepo userRepo) : ControllerBase
    {
        // ── DASHBOARD ─────────────────────────────────────────────────────────
        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var result = userRepo.GetDashboardData();
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // ── GET ALL USERS ─────────────────────────────────────────────────────
        // GET: api/admin/users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var result = userRepo.GetAll();
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // ── GET USER BY ID ────────────────────────────────────────────────────
        // GET: api/admin/users/5
        [HttpGet("users/{id}")]
        public IActionResult GetUserById(int id)
        {
            var result = userRepo.GetById(id);
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            if (result.Data == null)
                return NotFound(new { message = $"User#{id} not found" });

            return Ok(result.Data);
        }

        // ── CREATE USER ───────────────────────────────────────────────────────
        // POST: api/admin/users
        [HttpPost("users")]
        public IActionResult CreateUser([FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.ID = 0; // force create
            var result = userRepo.Save(model, GetLoggedInUserId());
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetUserById),
                new { id = result.Data!.ID },
                result.Data);
        }

        // ── UPDATE USER ───────────────────────────────────────────────────────
        // PUT: api/admin/users/5
        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User model)
        {
            if (id != model.ID)
                return BadRequest(new { message = "ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = userRepo.Save(model, GetLoggedInUserId());
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // ── DELETE USER ───────────────────────────────────────────────────────
        // DELETE: api/admin/users/5
        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = userRepo.Delete(id, GetLoggedInUserId());
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = $"User#{id} deleted successfully", data = result.Data });
        }

        // ── ACTIVITY LOGS ──────────────────────────────────────────────────────
        // GET: api/admin/activity-logs
        [HttpGet("activity-logs")]
        public IActionResult GetActivityLogs()
        {
            var result = userRepo.GetActivityLogs();
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // ── MANAGE USER ROLE ───────────────────────────────────────────────────
        // PATCH: api/admin/users/5/role
        [HttpPatch("users/{id}/role")]
        public IActionResult UpdateRole(int id, [FromBody] RoleUpdateRequest request)
        {
            var getResult = userRepo.GetById(id);
            if (getResult.HasError || getResult.Data == null)
                return NotFound(new { message = $"User#{id} not found" });

            getResult.Data.Role = request.Role;
            var result = userRepo.Save(getResult.Data, GetLoggedInUserId());
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = $"User#{id} role updated to {request.Role}", data = result.Data });
        }

        // ── TOGGLE ACTIVE STATUS ───────────────────────────────────────────────
        // PATCH: api/admin/users/5/status
        [HttpPatch("users/{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            var getResult = userRepo.GetById(id);
            if (getResult.HasError || getResult.Data == null)
                return NotFound(new { message = $"User#{id} not found" });

            getResult.Data.IsActive = request.IsActive;
            var result = userRepo.Save(getResult.Data, GetLoggedInUserId());
            if (result.HasError)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = $"User#{id} status updated to {(request.IsActive ? "Active" : "Inactive")}", data = result.Data });
        }

        // ── HELPER ────────────────────────────────────────────────────────────
        private int GetLoggedInUserId()
        {
            var val = User.FindFirst("UserId")?.Value;
            return int.TryParse(val, out int id) ? id : 0;
        }
    }

    // ── Request DTOs ──────────────────────────────────────────────────────────
    public class RoleUpdateRequest
    {
        public string Role { get; set; } = null!;
    }

    public class StatusUpdateRequest
    {
        public bool IsActive { get; set; }
    }
}
