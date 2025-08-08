using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using MediatR;

namespace Application.Commands.SendVerificationLink;

public class SendVerificationLinkHandler(IVerificationService service)
    : IRequestHandler<SendVerificationLinkCommand, SendVerificationLinkResult>
{
    public Task<SendVerificationLinkResult> Handle(SendVerificationLinkCommand request, CancellationToken cancellationToken)
        => service.SendVerificationLinkAsync(request.Email, request.IsEmailVerification, cancellationToken);
}
