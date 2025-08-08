using Application.Responses;

namespace Application.Abstractions.Security;

public interface IVerificationService
{
    Task<SendVerificationLinkResult> SendVerificationLinkAsync(string email, bool isEmailVerification,
                                                            CancellationToken cancellationToken);
    //todo move to the another service
    Task<VerifyEmailResult> VerifyEmailAsync(string token, CancellationToken cancellationToken);
    
}
