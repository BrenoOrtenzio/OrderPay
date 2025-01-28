using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using SESDemo.Models;

namespace SESDemo.Services;

public class SdkEmailService : IEmailService
{
    private readonly MailOptions _mailOptions;
    private readonly IAmazonSimpleEmailService _mailService;
    public SdkEmailService(IOptions<MailOptions> mailOptions,
        IAmazonSimpleEmailService mailService)
    {
        _mailOptions = mailOptions.Value;
        _mailService = mailService;
    }
    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var mailBody = new Body(new Content(mailRequest.Body));
        var message = new Message(new Content(mailRequest.Subject), mailBody);
        var destination = new Destination(new List<string> { mailRequest.Recepient! });
        var request = new SendEmailRequest(_mailOptions.Mail, destination, message);
        await _mailService.SendEmailAsync(request);
    }
}