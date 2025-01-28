using SESDemo.Models;

namespace SESDemo.Services;

public interface IEmailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}
