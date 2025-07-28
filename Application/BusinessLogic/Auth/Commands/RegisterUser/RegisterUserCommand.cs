using MediatR;

namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public record RegisterUserCommand(RegisterUserDto dto) : IRequest<Guid>;
