using MediatR;

namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;
