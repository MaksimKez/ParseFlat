using Application.Dtos;
using Application.Dtos.Users;
using Refit;

namespace Infrastructure.Clients.Interfaces;

public interface IUserServiceApi
{
    [Get("/UserProfile/{id}")]
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    [Get("/UserProfile/by-email")]
    Task<UserDto> GetByEmailAsync([Query] string email, CancellationToken cancellationToken = default);

    [Post("/UserProfile")]
    Task<UserDto> AddUserAsync([Body] AddUserProfileRequest request, CancellationToken cancellationToken = default);
}
