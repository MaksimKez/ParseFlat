using Application.Abstractions.EmailService;
using Application.Responses;
using MediatR;

namespace Application.Commands.SendVerificationLink;

public class SendVerificationLinkHandler(IEmailVerificationService service)
    : IRequestHandler<SendVerificationLinkCommand, SendVerificationLinkResult>
{
    public Task<SendVerificationLinkResult> Handle(SendVerificationLinkCommand request, CancellationToken cancellationToken)
    {
        return service.SendVerificationLinkAsync(request.Email, cancellationToken);
    }
}
