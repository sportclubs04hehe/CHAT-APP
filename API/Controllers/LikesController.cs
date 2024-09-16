using API.DTOs.User;
using API.Extensions;
using API.Helpers;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseController
    {
        [HttpPost("{targetUserId}")]
        public async Task<ActionResult> ToggleLike(string targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if (sourceUserId == targetUserId) BadRequest("Bạn không thể thích chính mình");

            try
            {
                var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

                if (existingLike == null)
                {
                    var like = new UserLike
                    {
                        SourceUserId = sourceUserId,
                        TargetUserId = targetUserId,
                    };

                    likesRepository.AddLike(like);
                }
                else
                {
                    likesRepository.DeleteLike(existingLike);
                }

                if (await likesRepository.SaveChange()) return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Có lỗi xảy ra, vui lòng thử lại sau.");
            }
            
            return BadRequest("Thích thất bại");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<string>>> GetCurrentUserLikeIds()
        {
            return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery]LikesParams? likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users);

            return Ok(users);
        }

        [HttpGet("liked-count/{userId}")]
        public async Task<ActionResult<int>> GetLikedCount(string userId)
        {
            var count = await likesRepository.GetLikedCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("liked-by-count/{userId}")]
        public async Task<ActionResult<int>> GetLikedByCount(string userId)
        {
            var count = await likesRepository.GetLikedByCountAsync(userId);
            return Ok(count);
        }
    }
}
