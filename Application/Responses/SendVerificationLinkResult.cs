namespace Application.Responses;

public class SendVerificationLinkResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static SendVerificationLinkResult Success() => new() { IsSuccess = true };
    public static SendVerificationLinkResult Failure(string errorMessage) 
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}