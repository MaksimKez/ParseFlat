using Application.Dtos.Users;
using Application.Responses;

namespace Application.Abstractions.UserService;

public interface IUserServiceClient
{
    Task<UserServiceResult> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserServiceResult> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserServiceResult> AddUserAsync(UserDto userDto, CancellationToken cancellationToken);
}