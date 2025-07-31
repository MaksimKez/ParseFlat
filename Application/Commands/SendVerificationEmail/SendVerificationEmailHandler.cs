using Application.Responses;
using MediatR;

namespace Application.Commands.SendVerificationEmail;

public class SendVerificationEmailHandler : IRequestHandler<SendVerificationEmailCommand, SendVerificationEmailResult>
{
    public Task<SendVerificationEmailResult> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}