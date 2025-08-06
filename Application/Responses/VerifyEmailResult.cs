namespace Application.Responses;

public class VerifyEmailResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static VerifyEmailResult Success()
        => new() { IsSuccess = true };
    public static VerifyEmailResult Failure(string errorMessage) 
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}