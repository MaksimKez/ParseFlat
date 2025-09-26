using Application.Responses;

namespace Application.Abstractions.Security;

public interface IVerificationService
{
    Task<Result> SendVerificationLinkAsync(
        string name,
        bool isEmailVerification,
        CancellationToken cancellationToken);
}
