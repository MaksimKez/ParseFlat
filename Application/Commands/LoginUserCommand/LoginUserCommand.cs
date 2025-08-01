using Application.Dtos.Users;
using Application.Responses;
using MediatR;

namespace Application.Commands.LoginUserCommand;

public record class LoginUserCommand(LoginUserRequest Dto) : IRequest<LoginUserResult>;