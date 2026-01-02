using EmailSenderApp.Models;
using EmailSenderApp.Options;
using System.Net;
using System.Net.Mail;

namespace EmailSenderApp.Services;

public class EmailSender : IEmailSender
{
    public readonly string address;
    public readonly string credential;

    public EmailSender(ConfigurationSettings configurationSettings)
    {
        address = configurationSettings.EmailSenderAddress;
        credential = configurationSettings.EmailSenderCredential;
    }


    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        using var client = new SmtpClient("smtp.gmail.com")
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(address, credential),
            Port = 587,
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(address, emailRequest.Name, System.Text.Encoding.UTF8),
            Subject = emailRequest.Subject,
            Body = emailRequest.Message,
            IsBodyHtml = true
        };

        message.To.Add(emailRequest.To);

        if (!string.IsNullOrWhiteSpace(emailRequest.From))
            message.ReplyToList.Add(new MailAddress(emailRequest.From));

        await client.SendMailAsync(message);
    }
}
