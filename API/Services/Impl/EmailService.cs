using API.DTOs.Account;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;

namespace API.Services.Impl
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        public async Task<bool> SendEmailAsync(EmailSendDto emailSendDto)
        {
            MailjetClient client = new MailjetClient(config["MailJet:ApiKey"], config["MailJet:SecretKey"]);

            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(config["Email:From"], config["Email:ApplicationName"]))
                .WithSubject(emailSendDto.Subject)
                .WithHtmlPart(emailSendDto.Body)
                .WithTo(new SendContact(emailSendDto.To))
                .Build();

            var response = await client.SendTransactionalEmailAsync(email);

            if (response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
