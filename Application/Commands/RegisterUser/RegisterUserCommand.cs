using Application.Dtos.Users;
using Application.Responses;
using MediatR;

namespace Application.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserRequest Dto) : IRequest<RegisterUserResponse>;
