using Application.Abstractions.Security;
using Application.Responses;
using MediatR;

namespace Application.Commands.ResetPassword;

public class PasswordResetHandler(IPasswordResetService service) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        => await service.ResetPasswordAsync(command.Request.Token, command.Request.NewPassword, cancellationToken);
}