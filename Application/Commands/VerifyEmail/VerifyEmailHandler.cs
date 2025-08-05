using Application.Abstractions.Messaging;
using Application.Responses;
using Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.VerifyEmail;

public class VerifyEmailHandler(
    IUnitOfWork unitOfWork,
    ILogger<VerifyEmailHandler> logger)
    : IRequestHandler<VerifyEmailCommand, VerifyEmailResult>,
        ITransactionalCommand<VerifyEmailResult>
{
    public async Task<VerifyEmailResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var tokenValue = request.Token;

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
        
        await unitOfWork.Users.UpdateAsync(user, cancellationToken);
        unitOfWork.EmailVerificationTokens.Update(verification, cancellationToken);

        logger.LogInformation("User {UserId} successfully verified email", user.Id);
        return VerifyEmailResult.Success();
    }
}
