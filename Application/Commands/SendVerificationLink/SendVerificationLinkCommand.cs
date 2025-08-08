using Application.Abstractions.Messaging;
using Application.Responses;

namespace Application.Commands.SendVerificationLink;

public record SendVerificationLinkCommand(string Email, bool IsEmailVerification) : ITransactionalCommand<SendVerificationLinkResult>;