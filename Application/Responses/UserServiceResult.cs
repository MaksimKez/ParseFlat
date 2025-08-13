using Application.Dtos.Users;

namespace Application.Responses;

public class UserServiceResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public UserDto? User { get; set; }
}