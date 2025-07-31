using Application.Abstractions.EmailService;
using Application.Responses.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task<EmailServiceResponse> SendEmailAsync(string toEmail, string toName, string token,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Sending email to {toEmail} with token {token}");

        //temp
        await Task.Delay(300, cancellationToken);
        return new EmailServiceResponse
        {
            ErrorMessage = null,
            IsSuccess = true,
            ToMail = toEmail
        };
    }
}