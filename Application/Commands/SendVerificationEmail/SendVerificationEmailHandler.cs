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
        var verificationToken = tokenGenerator.GenerateToken();

        logger.LogInformation("Sending verification email to {Email}", dto.Email);

        var result = await emailService.SendTokenEmailAsync(dto.Email, dto.Name, verificationToken, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to send verification email to {Email}: {Error}", dto.Email, result.ErrorMessage);
                                                                            //just to see in debug
            return SendVerificationEmailResult.Failed(result.ErrorMessage ?? throw new Exception(), dto.Email);
        }

        using var transaction = await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            var tokenModel = EmailVerificationToken.CreateNew(verificationToken, dto.UserId);
            await uow.EmailVerificationTokens.AddAsync(tokenModel, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await uow.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Verification token saved for {Email}", dto.Email);
            return SendVerificationEmailResult.Success(dto.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving verification token for {Email}, rolling back", dto.Email);
            await uow.RollbackTransactionAsync(cancellationToken);
            return SendVerificationEmailResult.Failed("Internal error occurred while saving token", dto.Email);
        }
    }
}
