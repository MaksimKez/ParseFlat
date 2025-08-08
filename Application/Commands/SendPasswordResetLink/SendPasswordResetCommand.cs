using Application.Abstractions.Messaging;
using Application.Dtos;
using Application.Responses;

namespace Application.Commands.SendPasswordResetLink;

public record SendPasswordResetCommand(string Email) : ITransactionalCommand<SendPasswordResetResult>;
