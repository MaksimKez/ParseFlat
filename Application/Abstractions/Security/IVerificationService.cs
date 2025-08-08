using Application.Responses;

namespace Application.Abstractions.Security;

public interface IVerificationService
{
    Task<SendVerificationLinkResult> SendVerificationLinkAsync(string email, bool isEmailVerification,
                                                            CancellationToken cancellationToken);
    
}
