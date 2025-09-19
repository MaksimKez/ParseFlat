namespace Application.Responses;

public class RefreshTokenResult : Result
{ 
    public string? Token { get; private init; }
    
    public static RefreshTokenResult Success(string token) 
        => new() { IsSuccess = true, Token = token };
    public new static RefreshTokenResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}