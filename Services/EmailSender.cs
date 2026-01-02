using EmailSenderApp.Models;
using EmailSenderApp.Options;
using System.Net;
using System.Net.Mail;

namespace EmailSenderApp.Services;

public class EmailSender : IEmailSender
{
    private readonly string _address;
    private readonly string _credential;

    public EmailSender(ConfigurationSettings configurationSettings)
    {
        _address = configurationSettings.EmailSenderAddress;
        _credential = configurationSettings.EmailSenderCredential;
    }


    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        using var client = new SmtpClient("smtp.gmail.com")
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_address, _credential),
            Port = 587,
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_address, emailRequest.Name, System.Text.Encoding.UTF8),
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
