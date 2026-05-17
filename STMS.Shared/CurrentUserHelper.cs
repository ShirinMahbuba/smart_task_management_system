using System.Security.Claims;

namespace STMS.Shared
{
    public static class CurrentUserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var val = user.FindFirst("UserId")?.Value;
            return int.TryParse(val, out int id) ? id : 0;
        }

        public static string GetUserName(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        }

        public static string GetUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "Viewer";
        }

        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
    }
}
