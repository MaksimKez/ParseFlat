using Application.Responses.Infrastructure;

namespace Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<EmailServiceResult> SendTokenEmailAsync(string toEmail,
        string toName, string token, CancellationToken cancellationToken = default);
}
