using Application.Abstractions.UserService;
using Application.Dtos.Users;
using Application.Responses;

namespace Infrastructure.UserServiceClient;

public class UserServiceClient : IUserServiceClient
{
    public async Task<UserServiceResult> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        
        //it is used in 1 scenario, so user - null for positive registration
        return new UserServiceResult()
        {
            ErrorMessage = "Not found",
            IsSuccess = false,
            User = null
        };
    }

    public async Task<UserServiceResult> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        return new UserServiceResult()
        {
            ErrorMessage = null,
            IsSuccess = true,
            User = new UserDto()
            {
                Email = "email@gmail.com",
                Id = id
            }
        };
    }

    public async Task<UserServiceResult> AddUserAsync(UserDto userDto, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        return new UserServiceResult()
        {
            ErrorMessage = null,
            IsSuccess = true,
            User = new UserDto()
            {
                Email = "email@gmail.com",
                Id = userDto.Id
            }
        };
    }
}