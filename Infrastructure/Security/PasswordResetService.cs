using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Email;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Security;

public class PasswordResetService(IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    IEmailService emailService,
    ILogger<EmailVerificationService> logger) : IPasswordResetService
{
    private const int passwordResetHours = 8; 
    public async Task<SendPasswordResetResult> SendPasswordResetTokenAsync(string email, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FindByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", email);
            return SendPasswordResetResult.Failure("User not found");
        }

        var token = tokenGenerator.GenerateToken();

        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(passwordResetHours)
        };

        await unitOfWork.PasswordResetTokens.AddAsync(passwordResetToken, cancellationToken);

        var emailResult = await emailService.SendEmailAsync(user.Email, user.Name, token, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            logger.LogError("Failed to send verification email to {Email}", user.Email);
            return SendPasswordResetResult.Failure("Failed to send password reset token");
        }

        SimulateEmailSent(token);
        logger.LogInformation("Verification email sent to {Email}", user.Email);
        return SendPasswordResetResult.Success();
    }

    private void SimulateEmailSent(string token)
    {
        Console.WriteLine(token);
        Console.WriteLine(token);
        Console.WriteLine(token);
    }
}