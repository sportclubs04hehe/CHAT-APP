using API.Extensions;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace API.Helpers
{
    #region Giải thích
    /*
     IAsyncActionFilter là một phần của ASP.NET Core Middleware cho phép bạn chèn logic vào 
    trước và sau khi một hành động trong controller được thực hiện.
     */
    #endregion
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LogUserActivity>>(); // lấy logger

            // Chạy middleware tiếp theo trong pipeline
            var resultContext = await next();

            // Log thông tin xác thực
            if (resultContext.HttpContext.User.Identity == null)
            {
                logger.LogInformation("User.Identity is null.");
                return;
            }

            logger.LogInformation("IsAuthenticated: {IsAuthenticated}", resultContext.HttpContext.User.Identity.IsAuthenticated);

            // Nếu người dùng chưa xác thực thì return
            if (resultContext.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                logger.LogInformation("User is not authenticated.");
                return;
            }

            // Lấy userId từ claims
            var userId = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("User ID: {UserId}", userId);

            if (userId == null)
            {
                logger.LogWarning("User ID claim is missing.");
                return;
            }

            // Lấy UserManager từ DI container
            var userManager = resultContext.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByIdAsync(userId); // sử dụng FindByIdAsync thay vì FindByNameAsync

            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userId);
                return;
            }

            // Cập nhật last active
            user.LastActive = DateTime.Now;
            var result = await userManager.UpdateAsync(user);

            // Log kết quả của việc cập nhật
            if (result.Succeeded)
            {
                logger.LogInformation("User {UserId} last active time updated successfully.", userId);
            }
            else
            {
                logger.LogError("Failed to update last active time for User {UserId}.", userId);
            }
        }
    }
}
