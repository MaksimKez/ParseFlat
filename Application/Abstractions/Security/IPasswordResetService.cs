using Application.Responses;

namespace Application.Abstractions.Security;

public interface IPasswordResetService
{
    Task<SendPasswordResetResult> SendPasswordResetTokenAsync(string email, CancellationToken cancellationToken);
}