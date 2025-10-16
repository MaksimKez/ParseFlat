using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface IEmailVerifier
{
    Task<Result> VerifyEmailAsync(string token, CancellationToken cancellationToken);
}