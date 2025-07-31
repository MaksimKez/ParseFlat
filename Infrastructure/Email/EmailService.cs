using System.Data;
using Application.Abstractions.EmailService;
using Application.Responses.Infrastructure;
using Infrastructure.Email.Configs;
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
        logger.LogInformation($"Building email to {toEmail} with token {token}");

        var emailJObject = emailBuilder
            .SetSubject("EmailVerification")
            .SetTo(toEmail, toName)
            .SetFrom(mailSettings.FromEmail, mailSettings.FromName)
            .SetTextBody(token)   // temporary, later i will add endpoint for this
            .Build();
        
        logger.LogInformation($"Email is built to email to {toEmail} with token {token}");
        
        var result = await emailSender.SendAsync(emailJObject, cancellationToken);

        return result.IsSuccess ?
            EmailServiceResult.Success(toEmail) :           //exception is only for debugging, will be removed
            EmailServiceResult.Failure(result.ErrorMessage ?? throw new InvalidExpressionException(), toEmail);
    }
}

