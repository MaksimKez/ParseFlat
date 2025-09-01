using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Abstractions.UserService;
using Application.Dtos;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class EmailVerificationService(
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    INotificationServiceClient notificationServiceClient,
    IUserServiceClient userServiceClient,
    ILogger<EmailVerificationService> logger)
    : IEmailVerificationService
{

    private const int emailVerificationTokeHours = 24; 
    public async Task<SendVerificationLinkResult> SendVerificationLinkAsync(string name, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FindByNameAsync(name, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with name {Name} not found", name);
            return SendVerificationLinkResult.Failure("User not found");
        }
        
        var result = await userServiceClient.FindByIdAsync(user.Id, cancellationToken);
        if (result.User is null)
        {
            logger.LogWarning("User with name {Name} not found", name);
            return SendVerificationLinkResult.Failure(result.ErrorMessage!);
        }

        var token = tokenGenerator.GenerateToken();

        var emailVerification = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(emailVerificationTokeHours)
        };

        await unitOfWork.EmailVerificationTokens.AddAsync(emailVerification, cancellationToken);

        var emailResult = await notificationServiceClient.SendEmailAsync(new EmailCodeDto
        {
            ToEmail = result.User.Email,
            ToName = user.Name,
            Token = token
        }, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            logger.LogError("Failed to send verification name to {Email}", result.User.Email);
            return SendVerificationLinkResult.Failure("Failed to send verification name");
        }

        SimulateEmailSent(token);
        logger.LogInformation("Verification name sent to {Email}", result.User.Email);
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