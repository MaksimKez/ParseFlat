using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface IEmailVerificationService
{
    Task<SendVerificationLinkResult> SendVerificationLinkAsync(string name, CancellationToken cancellationToken);
    Task<VerifyEmailResult> VerifyEmailAsync(string token, CancellationToken cancellationToken);
}
