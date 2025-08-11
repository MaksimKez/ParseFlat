using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface IEmailVerifier
{
    Task<VerifyEmailResult> VerifyEmailAsync(string token, CancellationToken cancellationToken);
}