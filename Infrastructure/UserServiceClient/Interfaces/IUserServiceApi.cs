using Application.Dtos;
using Application.Dtos.Users;
using Refit;

namespace Infrastructure.UserServiceClient.Interfaces;

public interface IUserServiceApi
{
    [Get("/{id}")]
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    [Get("/by-email")]
    Task<UserDto> GetByEmailAsync([Query] string email, CancellationToken cancellationToken = default);

    [Post("/")]
    Task<UserDto> AddUserAsync([Body] AddUserProfileRequest request, CancellationToken cancellationToken = default);
}
