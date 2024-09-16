using API.DTOs.Message;
using API.Extensions;
using API.Helpers;
using API.Models;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController(IMessageRepository messageRepository,
        IMapper mapper,
        IUserRepository userRepository) : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] CreateMessageDto createMessageDto)
        {
            try
            {
                var username = User.GetUsername();

                if (username == createMessageDto.RecipientUsername.ToLower())
                {
                    return BadRequest("Bạn không thể gửi tin nhắn cho chính mình");
                }

                var sender = await userRepository.GetUserByUsernameAsync(username);
                var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

                if (recipient == null || sender == null)
                {
                    return BadRequest("Không thể gửi tin nhắn vào thời điểm này");
                }

                var message = new Messages
                {
                    Sender = sender,
                    Recipient = recipient,
                    SenderUsername = sender.UserName,
                    RecipientUsername = recipient.UserName,
                    Content = createMessageDto.Content,
                };

                messageRepository.AddMessage(message);

                if (await messageRepository.SaveAllAsync())
                {
                    return Ok(mapper.Map<MessageDto>(message));
                }

                return BadRequest("Lỗi khi lưu tin nhắn");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            try
            {
                messageParams.Username = User.GetUsername();

                var messages = await messageRepository.GetMessagesForUserAsync(messageParams);

                Response.AddPaginationHeader(messages);

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            try
            {
                var currentUsername = User.GetUsername();
                return Ok(await messageRepository.GetMessagesThreadAsync(currentUsername, username));
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteMessage(Guid id)
        {
            try
            {
                var username = User.GetUsername();

                var message = await messageRepository.GetMessagesAsync(id);

                if (message == null) return BadRequest("Không thể xóa tin nhắn này");

                if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

                if (message.SenderUsername == username) message.SenderDeleted = true;

                if (message.RecipientUsername == username) message.RecipientDeleted = true;

                if (message is { SenderDeleted: true, RecipientDeleted: true })
                {
                    messageRepository.DeleteMessage(message);
                }

                if (await messageRepository.SaveAllAsync()) return Ok();

                return BadRequest("Có lỗi xảy ra khi xóa tin nhắn");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }


    }
}
