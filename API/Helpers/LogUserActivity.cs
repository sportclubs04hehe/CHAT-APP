using API.Extensions;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

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
            // Thực thi logic trước khi hành động trong controller được gọi
            //-----------------------------------------------------//
            // Thực thi hành động trong controller
            var resultContext = await next();

            // Kiểm tra xem người dùng có được xác thực hay không
            if (resultContext.HttpContext.User.Identity?.IsAuthenticated != true) return;

            // Lấy userId từ ClaimsPrincipal
            var userId = resultContext.HttpContext.User.GetUserId();

            // Lấy IUnitOfWork từ dịch vụ yêu cầu
            var uow = resultContext.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

            var user = await uow.FindByNameAsync(userId);

            if (user == null) return;

            user.LastActive = DateTime.Now;

            await uow.UpdateAsync(user);
        }
    }
}
