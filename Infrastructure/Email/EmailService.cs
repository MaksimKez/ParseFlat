using Application.Abstractions.EmailService;
using Application.Responses.Infrastructure;
using Infrastructure.Email.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

public class EmailService(
    ILogger<EmailService> logger,
    IEmailBuilder emailBuilder,
    IOptions<MailSettings> mailSettingsOptions,
    IEmailSender emailSender) : IEmailService
{
    private readonly MailSettings mailSettings = mailSettingsOptions.Value;
    
    public async Task<EmailServiceResult> SendTokenEmailAsync(string toEmail, string toName, string token,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Sending email to {toEmail} with token {token}");

        var emailJObject = emailBuilder
            .SetSubject("EmailVerification")
            .SetTo(toEmail, toName)
            .SetFrom(mailSettings.FromEmail, mailSettings.FromName)
            .SetTextBody(token)   // temporary, later i will add endpoint for this
            .Build();
        
        await emailSender.SendAsync(emailJObject, cancellationToken);
        
        return new EmailServiceResult
        {
            ErrorMessage = null,
            IsSuccess = true,
            ToMail = toEmail
        };
    }
}

public class MailSettings
{
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string EmployeeEmail { get; set; }
}