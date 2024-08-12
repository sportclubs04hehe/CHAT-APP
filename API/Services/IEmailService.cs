using API.DTOs.Account;

namespace API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailSendDto emailSendDto);
    }
}
