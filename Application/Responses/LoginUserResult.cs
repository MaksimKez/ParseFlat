namespace Application.Responses;

public class LoginUserResult : Result
{ 
    public string UserEmail { get; init; }
    public string RefreshToken { get; private init; }
    public string AccessToken { get; private init; }

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
    
    public new static LoginUserResult Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
