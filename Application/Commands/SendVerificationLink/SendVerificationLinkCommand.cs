using Application.Abstractions.Messaging;
using Application.Responses;

namespace Application.Commands.SendVerificationLink;

public record SendVerificationLinkCommand(string Name, bool IsEmailVerification) : ITransactionalCommand<Result>;
