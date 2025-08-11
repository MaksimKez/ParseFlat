using Application.Abstractions.EmailService;
using Application.Abstractions.Security;
using Application.Responses;
using MediatR;

namespace Application.Commands.VerifyEmail;

public class VerifyEmailHandler(IEmailVerifier service)
    : IRequestHandler<VerifyEmailCommand, VerifyEmailResult>
{
    public Task<VerifyEmailResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) 
        => service.VerifyEmailAsync(request.Token, cancellationToken);
}
