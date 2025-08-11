using Application.Responses;

namespace Application.Abstractions.Security;

public interface IPasswordResetService
{
    Task<PasswordResetResult> ResetPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken);
}