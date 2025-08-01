using Domain.Entities;

namespace Application.Responses;

public class LoginUserResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string UserEmail { get; init; }

    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }

    public static LoginUserResult Success(string email, string refreshToken, string accessToken)
    {
        return new LoginUserResult
        {
            IsSuccess = true,
            ErrorMessage = null,
            UserEmail = email,
            RefreshToken = refreshToken,
            AccessToken = accessToken
        };
    }
    
    public static LoginUserResult Failure(string error) => new LoginUserResult { IsSuccess = false, ErrorMessage = error };
}