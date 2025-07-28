using Application.Responses;
using MediatR;

namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserDto Dto) : IRequest<RegisterUserResponse>;
