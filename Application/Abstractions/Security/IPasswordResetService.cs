using Application.Responses;

namespace Application.Abstractions.Security;

public interface IPasswordResetService
{
    Task<Result> ResetPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken);
}
