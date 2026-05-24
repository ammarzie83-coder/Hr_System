using System.Security.Claims;

namespace Hr_System.Data
{
    public static class AuthorizationExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal? user, string permission)
        {
            if (user == null)
            {
                return false;
            }

            return user.Claims.Any(c => c.Type == "Permission" && c.Value == permission);
        }

        public static string GetDisplayName(this ClaimsPrincipal user)
        {
            return user.Identity?.Name ?? string.Empty;
        }
    }
}
