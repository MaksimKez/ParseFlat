using Application.Responses;

namespace Application.Abstractions.Security;

public interface IVerificationService
{
    Task<SendVerificationLinkResult> SendVerificationLinkAsync(
        string name,
        bool isEmailVerification,
        CancellationToken cancellationToken);
}
