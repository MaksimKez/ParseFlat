using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface IEmailVerificationService
{
    Task<Result> SendVerificationLinkAsync(string name, CancellationToken cancellationToken);
    Task<Result> VerifyEmailAsync(string token, CancellationToken cancellationToken);
}
