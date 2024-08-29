using API.DTOs.User;
using API.Extensions;
using API.Models;
using API.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper,
        IPhotoService photoService) : BaseController
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
            try
            {
                var user = await userManager.FindByNameAsync(User.GetUsername());
                if (user == null)
                {
                    return NotFound("Người dùng không tồn tại");
                }

                mapper.Map(memberUpdate, user);

                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("Cập nhật không thành công");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            try
            {
                var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("Không tìm thấy người dùng");

                var result = await photoService.AddPhotoAsync(file);

                if (result.Error != null) return BadRequest(result.Error.Message);

                var photo = new Photo
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId
                };

                if (user.Photos.Count == 0) photo.IsMain = true;

                user.Photos.Add(photo);

                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                    return CreatedAtAction(nameof(GetUserByUserName),
                        new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

                return BadRequest("Có vấn đề xảy ra khi thêm ảnh");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("set-main-photo/{photoId:guid}")]
        public async Task<ActionResult> SetMainPhoto(Guid photoId)
        {
            try
            {
                var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

                if (user == null) return BadRequest("Không tìm thấy người dùng");

                var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);


                if (photo == null || photo.IsMain) return BadRequest("Ảnh này đã là ảnh đại diện");

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if (currentMain != null) currentMain.IsMain = false;

                photo.IsMain = true;

                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded) return NoContent();

                return BadRequest("Có vấn đề xảy ra khi cập nhật ảnh đại diện");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        //[HttpDelete("delete-photo/{photoId:guid}")]
        //public async Task<ActionResult> DeletePhoto(Guid photoId)
        //{
        //    try
        //    {
        //        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        //        if (user == null) return BadRequest("User not found");

        //        var photo = await photoRepository.GetPhotoById(photoId);

        //        if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

        //        if (photo.PublicId != null)
        //        {
        //            var result = await photoService.DeletePhotoAsync(photo.PublicId);
        //            if (result.Error != null) return BadRequest(result.Error.Message);
        //        }

        //        user.Photos.Remove(photo);

        //        if (await unitOfWork.Complete()) return Ok();

        //        return BadRequest("Có vấn đề xảy ra khi xóa ảnh");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        //    }
        //}
    }
}
