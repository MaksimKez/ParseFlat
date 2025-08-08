using Application.Abstractions.Security;
using Application.Responses;
using MediatR;

namespace Application.Commands.ResetPassword;

public class PasswordResetHandler(IPasswordResetService service) : IRequestHandler<ResetPasswordCommand, PasswordResetResult>
{
    public async Task<PasswordResetResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        => await service.ResetPasswordAsync(command.Request.Token, command.Request.NewPassword, cancellationToken);
}