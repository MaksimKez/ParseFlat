using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class EmailVerificationService(
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    IEmailService emailService,
    ILogger<EmailVerificationService> logger)
    : IEmailVerificationService
{

    private const int emailVerificationTokeHours = 24; 
    public async Task<SendVerificationLinkResult> SendVerificationLinkAsync(string email, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FindByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return SendVerificationLinkResult.Failure("User not found");
        }

        var token = tokenGenerator.GenerateToken();

        var emailVerification = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(emailVerificationTokeHours)
        };

        await unitOfWork.EmailVerificationTokens.AddAsync(emailVerification, cancellationToken);

        var emailResult = await emailService.SendEmailAsync(user.Email, user.Name, token, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            logger.LogError("Failed to send verification email to {Email}", user.Email);
            return SendVerificationLinkResult.Failure("Failed to send verification email");
        }

        SimulateEmailSent(token);
        logger.LogInformation("Verification email sent to {Email}", user.Email);
        return SendVerificationLinkResult.Success();
    }

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

    private void SimulateEmailSent(string token)
    {
        Console.WriteLine(token);
        Console.WriteLine(token);
        Console.WriteLine(token);
    }
}