namespace Application.Responses;

public class AuthHelperResult : Result
{
    public string? Value { get; private init; }
    
    public static AuthHelperResult Success(string value) => new() { IsSuccess = true, Value = value };
    public new static AuthHelperResult Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}