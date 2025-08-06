using Application.Abstractions.EmailService;
using Application.Responses;
using MediatR;

namespace Application.Commands.VerifyEmail;

public class VerifyEmailHandler(IEmailVerificationService service)
    : IRequestHandler<VerifyEmailCommand, VerifyEmailResult>
{
    public Task<VerifyEmailResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) 
        => service.VerifyEmailAsync(request.Token, cancellationToken);
}