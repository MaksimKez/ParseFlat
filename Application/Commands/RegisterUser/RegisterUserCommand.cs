using Application.Abstractions.Messaging;
using Application.Dtos.Users;
using Application.Responses;
using MediatR;

namespace Application.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserRequest Request) : ITransactionalCommand<RegisterUserResult>;
