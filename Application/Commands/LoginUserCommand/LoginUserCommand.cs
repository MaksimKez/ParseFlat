using Application.Abstractions.Messaging;
using Application.Dtos.Users;
using Application.Responses;
using MediatR;

namespace Application.Commands.LoginUserCommand;

public record LoginUserCommand(LoginUserRequest Request) : ITransactionalCommand<LoginUserResult>;
