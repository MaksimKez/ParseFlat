using Application.Abstractions.EmailService;
using Application.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.SendVerificationEmail;

public class SendVerificationEmailHandler : IRequestHandler<SendVerificationEmailCommand, SendVerificationEmailResult>
{
    private readonly IVerificationEmailFacade _verificationEmailFacade;
    private readonly ILogger<SendVerificationEmailHandler> _logger;

    public SendVerificationEmailHandler(
        IVerificationEmailFacade verificationEmailFacade,
        ILogger<SendVerificationEmailHandler> logger)
    {
        _verificationEmailFacade = verificationEmailFacade;
        _logger = logger;
    }

    public async Task<SendVerificationEmailResult> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling SendVerificationEmailCommand for userId: {UserId}, email: {Email}",
            request.Dto.UserId, request.Dto.Email);

        var result = await _verificationEmailFacade.SendVerificationEmailAsync(
            request.Dto.UserId,
            request.Dto.Email,
            request.Dto.Name,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Verification email sent successfully to {Email}", result.ToEmail);
        }
        else
        {
            _logger.LogWarning("Failed to send verification email to {Email}: {ErrorMessage}", result.ToEmail, result.ErrorMessage);
        }

        return result;
    }
}
