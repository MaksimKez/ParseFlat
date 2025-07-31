using Application.Responses;

namespace Application.Abstractions.EmailService;

public interface IVerificationEmailFacade
{
    Task<SendVerificationEmailResult> SendVerificationEmailAsync(
        Guid userId, 
        string email, 
        string userName, 
        CancellationToken cancellationToken);
}
