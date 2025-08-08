using Application.Abstractions.Messaging;
using Application.Dtos;
using Application.Responses;

namespace Application.Commands.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequest Request) : ITransactionalCommand<PasswordResetResult>;