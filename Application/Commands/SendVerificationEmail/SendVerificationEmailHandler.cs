using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.SendVerificationEmail;

public class SendVerificationEmailHandler(
    IEmailService emailService,
    IUnitOfWork uow,
    ILogger<SendVerificationEmailHandler> logger,
    ITokenGenerator tokenGenerator)
    : IRequestHandler<SendVerificationEmailCommand, SendVerificationEmailResult>
{
    public async Task<SendVerificationEmailResult> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        logger.LogDebug("Starting verification email process for userId: {UserId}, email: {Email}", dto.UserId, dto.Email);

        #region user exists and valid
        var user = await uow.Users.FindByIdAsync(dto.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found: {UserId}", dto.UserId);
            return SendVerificationEmailResult.Failed("User not found", dto.Email);
        }

        if (user.Email != dto.Email)
        {
            logger.LogWarning("Email mismatch for userId {UserId}: expected {ExpectedEmail}, got {ActualEmail}", dto.UserId, user.Email, dto.Email);
            return SendVerificationEmailResult.Failed("User not verified", dto.Email);
        }
        #endregion
        
        #region generate token
        var verificationToken = tokenGenerator.GenerateToken();
        #endregion
        logger.LogInformation("Generated verification token for userId: {UserId}", dto.UserId);

        logger.LogInformation("Sending verification email to {Email}", dto.Email);

        #region sending token

        var result = await emailService.SendTokenEmailAsync(dto.Email, dto.Name, verificationToken, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to send verification email to {Email}: {Error}", dto.Email, result.ErrorMessage);
            return SendVerificationEmailResult.Failed(result.ErrorMessage ?? "Unknown error", dto.Email);
        }

        #endregion

        logger.LogDebug("Email sent successfully to {Email}, starting transaction", dto.Email);

        #region adding and saving
        using var transaction = await uow.BeginTransactionAsync(cancellationToken);
        try
        {
            var tokenModel = EmailVerificationToken.CreateNew(verificationToken, dto.UserId);
            await uow.EmailVerificationTokens.AddAsync(tokenModel, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await uow.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Verification token saved and transaction committed for {Email}", dto.Email);

            return SendVerificationEmailResult.Success(dto.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving verification token for {Email}. Rolling back transaction.", dto.Email);
            await uow.RollbackTransactionAsync(cancellationToken);
            return SendVerificationEmailResult.Failed("Internal error occurred while saving token", dto.Email);
        }
        #endregion
    }
}


