using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController(Context context) : BaseController
    {
        [Authorize]
        [HttpGet("auth")] // 401
        public ActionResult<string> GetAuth()
        {
            return "secret text";
        }

        [HttpGet("not-found")] // 404
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = context.Users.Find("-1");

            if (thing == null) { return NotFound(); }

            return thing;
        }

        [HttpGet("server-error")] // 500
        public ActionResult<AppUser> GetServerError()
        {
            var thing = context.Users.Find(-1) ?? throw new Exception("Lỗi");

            return thing;
        }

        [HttpGet("bad-request")] // 400
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad Request");
        }

    }
}
