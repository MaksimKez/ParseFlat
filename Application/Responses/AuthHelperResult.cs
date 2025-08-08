namespace Application.Responses;

public class AuthHelperResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    /// <summary>
    /// <param name="value">Can be either email or token</param>
    /// </summary>
    public string? Value { get; init; }
    
    public static AuthHelperResult Success(string value) => new AuthHelperResult { IsSuccess = true, Value = value };
    public static AuthHelperResult Failure(string errorMessage) => new AuthHelperResult { IsSuccess = false, ErrorMessage = errorMessage };
}