using Application.Dtos.Users;
using Application.Responses;

namespace Application.Common.Interfaces;

public interface IUserService
{
    Task<RegisterUserResult> RegisterAsync(RegisterUserRequest dto, CancellationToken cancellationToken);
    Task<LoginUserResult> LoginAsync(LoginUserRequest dto, CancellationToken cancellationToken);
}