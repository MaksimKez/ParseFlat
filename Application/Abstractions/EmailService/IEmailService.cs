using Application.Responses.Infrastructure;

namespace Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<EmailServiceResponse> SendEmailAsync(string toEmail,
        string toName, string token, CancellationToken cancellationToken = default);
}
