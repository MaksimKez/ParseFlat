using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Security;

public class PasswordResetService(
    IUnitOfWork uow,
    IPasswordHasher hasher,
    ILogger<PasswordResetService> logger)
    : IPasswordResetService
{
    public async Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
    {
        var tokenModel = await uow.PasswordResetTokens.FindByTokenAsync(token, cancellationToken);

        if (tokenModel is null)
        {
            logger.LogWarning("Token not found {Token}", token);
            return Result.Failure("Token not found");
        }

        if (tokenModel.IsUsed || tokenModel.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Token is used or expired {Token}", token);
            return Result.Failure("Token is used or expired");
        }

        var user = tokenModel.User;
        if (user is null)
        {
            logger.LogWarning("User not found {User}", token);
            return Result.Failure("User data is missing");
        }
        
        user.PasswordHash = hasher.HashPassword(newPassword);
        tokenModel.IsUsed = true;
        
        logger.LogInformation("User {userId} successfully changed password", user.Id);
        return Result.Success();
    }
}