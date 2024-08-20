using API.DTOs.User;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(UserManager<AppUser> userManager,
        IUserRepository userRepository) : BaseController
    {
        // Get All Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetAllMemberAsync();

            return Ok(users);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<ActionResult<AppUser>> GetUserById([FromRoute]string id)
        {
            var user = await userRepository.GetUserByIdAsync(id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("get-by-username/{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByUserName([FromRoute] string username)
        {
            var user = await userRepository.GetMemberByUsernameAsync(username);

            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
