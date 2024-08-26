using API.DTOs.User;
using API.Models;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper) : BaseController
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

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] MemberUpdateDto memberUpdate)
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            if (user == null)
            {
                return BadRequest("Người dùng không tồn tại");
            }

            mapper.Map(memberUpdate, user);

            var result = await userManager.UpdateAsync(user);

            if(!result.Succeeded) return BadRequest("Cập nhật không thành công");

            return NoContent();
        }
    }
}
