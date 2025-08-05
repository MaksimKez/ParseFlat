namespace Application.Responses;

public class RefreshTokenResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Token { get; init; }
    
    public static RefreshTokenResult Success(string token) => new RefreshTokenResult { IsSuccess = true, Token = token };
    public static RefreshTokenResult Failure(string errorMessage) => new RefreshTokenResult { IsSuccess = false, ErrorMessage = errorMessage };
}