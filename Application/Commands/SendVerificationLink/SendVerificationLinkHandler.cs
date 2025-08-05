using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.SendVerificationLink;

public class SendVerificationLinkHandler : IRequestHandler<SendVerificationLinkCommand, SendVerificationLinkResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendVerificationLinkHandler> _logger;

    public SendVerificationLinkHandler(
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        ILogger<SendVerificationLinkHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<SendVerificationLinkResult> Handle(SendVerificationLinkCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", request.Email);
            return SendVerificationLinkResult.Failure("User not found");
        }

        var token = _tokenGenerator.GenerateToken();

        var emailVerification = new EmailVerificationToken()
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await _unitOfWork.EmailVerificationTokens.AddAsync(emailVerification, cancellationToken);
        
        var emailResult = await _emailService.SendEmailAsync(user.Email, user.Name, token, cancellationToken);

        if (!emailResult.IsSuccess)
        {
            _logger.LogError("Failed to send verification email to {Email}", user.Email);
            return SendVerificationLinkResult.Failure("Failed to send verification email");
        }

        SimulateEmailSent(token);
        _logger.LogInformation("Verification email sent to {Email}", user.Email);
        return SendVerificationLinkResult.Success();
    }

    private void SimulateEmailSent(string token)
    {
        Console.WriteLine(token);
        Console.WriteLine(token);
        Console.WriteLine(token);
    }
}
