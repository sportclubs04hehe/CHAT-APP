using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var username = user.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new Exception("Không thể lấy được username(email) từ token");

            return username;
        }
    }
}
