using Application.Abstractions.EmailService;
using Application.Responses;
using Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class EmailVerifier(IUnitOfWork unitOfWork, ILogger<EmailVerifier> logger) : IEmailVerifier
{
    
    public async Task<VerifyEmailResult> VerifyEmailAsync(string tokenValue, CancellationToken cancellationToken)
    {
        var verification = await unitOfWork.EmailVerificationTokens
            .FindByTokenAsync(tokenValue, cancellationToken);

        if (verification is null)
        {
            logger.LogWarning("Verification token not found: {Token}", tokenValue);
            return VerifyEmailResult.Failure("Invalid or expired token.");
        }

        if (verification.IsUsed || verification.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Verification token expired or already used: {Token}", tokenValue);
            return VerifyEmailResult.Failure("Token expired or already used.");
        }

        var user = verification.User;
        if (user is null)
        {
            logger.LogError("User not loaded for token: {Token}", tokenValue);
            return VerifyEmailResult.Failure("User data missing.");
        }

        user.IsVerified = true;
        verification.IsUsed = true;

        logger.LogInformation("User {UserId} successfully verified email", user.Id);
        return VerifyEmailResult.Success();
    }

}