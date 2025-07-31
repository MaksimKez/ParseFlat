using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public class VerificationEmailFacade : IVerificationEmailFacade
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<VerificationEmailFacade> _logger;

    public VerificationEmailFacade(
        IUnitOfWork uow,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        ILogger<VerificationEmailFacade> logger)
    {
        _uow = uow;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<SendVerificationEmailResult> SendVerificationEmailAsync(
        Guid userId,
        string email,
        string userName,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting verification email process for userId: {UserId}, email: {Email}", userId, email);

        var user = await _uow.Users.FindByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return SendVerificationEmailResult.Failed("User not found", email);
        }

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Email mismatch for userId {UserId}: expected {ExpectedEmail}, got {ActualEmail}", userId, user.Email, email);
            return SendVerificationEmailResult.Failed("User not verified", email);
        }

        var verificationToken = _tokenGenerator.GenerateToken();
        _logger.LogInformation("Generated verification token for userId: {UserId}", userId);

        var emailResult = await _emailService.SendTokenEmailAsync(email, userName, verificationToken, cancellationToken);
        if (!emailResult.IsSuccess)
        {
            _logger.LogWarning("Failed to send verification email to {Email}: {Error}", email, emailResult.ErrorMessage);
            return SendVerificationEmailResult.Failed(emailResult.ErrorMessage ?? "Unknown error", email);
        }

        _logger.LogDebug("Email sent successfully to {Email}, starting transaction", email);

        try
        {
            using var transaction = await _uow.BeginTransactionAsync(cancellationToken);

            var tokenModel = EmailVerificationToken.CreateNew(verificationToken, userId);
            await _uow.EmailVerificationTokens.AddAsync(tokenModel, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await _uow.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Verification token saved and transaction committed for {Email}", email);

            return SendVerificationEmailResult.Success(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving verification token for {Email}. Rolling back transaction.", email);
            await _uow.RollbackTransactionAsync(cancellationToken);
            return SendVerificationEmailResult.Failed("Internal error occurred while saving token", email);
        }
    }
}
