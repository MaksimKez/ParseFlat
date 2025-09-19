using Application.Dtos.Users;

namespace Application.Responses;

public class UserServiceResult : Result
{
    public UserDto? User { get; set; }
    
    public static UserServiceResult Success(UserDto user) => new() { IsSuccess = true, User = user };
}