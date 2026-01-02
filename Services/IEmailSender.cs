using EmailSenderApp.Models;

namespace EmailSenderApp.Services;

public interface IEmailSender
{
    Task SendEmailAsync(EmailRequest emailRequest);
}
