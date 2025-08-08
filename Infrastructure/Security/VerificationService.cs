using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Security;

public class VerificationService(
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    IEmailService emailService,
    ILogger<VerificationService> logger)
    : IVerificationService
{

    private const int emailVerificationTokeHours = 24; 
    public async Task<SendVerificationLinkResult> SendVerificationLinkAsync(string email, bool isEmailVerification,
                                                        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FindByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return SendVerificationLinkResult.Failure("User not found");
        }

        var token = tokenGenerator.GenerateToken();

        if (isEmailVerification)
        {
            await CreateEmailVerificationTokenAsync(user.Id, token, cancellationToken);
        }
        else
        {
            await CreatePasswordVerificationTokenAsync(user.Id, token, cancellationToken);
        }

        var emailResult = await emailService.SendEmailAsync(user.Email, user.Name, token, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            logger.LogError("Failed to send verification email to {Email}", user.Email);
            return SendVerificationLinkResult.Failure("Failed to send verification email");
        }

        logger.LogInformation("Verification email sent to {Email}", user.Email);
        return SendVerificationLinkResult.Success();
    }

    private async Task CreateEmailVerificationTokenAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        var emailVerificationToken = new EmailVerificationToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(emailVerificationTokeHours)
        };

        await unitOfWork.EmailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
    }

    private async Task CreatePasswordVerificationTokenAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        var passwordVerificationToken = new PasswordResetToken()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(emailVerificationTokeHours)
        };

        await unitOfWork.PasswordResetTokens.AddAsync(passwordVerificationToken, cancellationToken);
    }
}