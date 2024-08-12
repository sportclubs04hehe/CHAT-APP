using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PlaysController : BaseController
    {
        [Authorize]
        [HttpGet("get-all")]
        public IActionResult Players()  
        {
            return Ok(new JsonResult(new { message = "Chỉ có người dùng mới thấy được giao diện này" }));
        }

    }
}
