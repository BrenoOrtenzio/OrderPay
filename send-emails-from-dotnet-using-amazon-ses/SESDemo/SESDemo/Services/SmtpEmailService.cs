using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SESDemo.Models;

namespace SESDemo.Services;

public class SmtpEmailService : IEmailService
{
    private readonly MailOptions _mailOptions;
    public SmtpEmailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
    }
    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_mailOptions.DisplayName, _mailOptions.Mail));
        email.To.Add(MailboxAddress.Parse(mailRequest.Recepient));
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_mailOptions.Host, _mailOptions.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailOptions.Username, _mailOptions.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}
