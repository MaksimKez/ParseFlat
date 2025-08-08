
namespace Application.Responses;

public class SendPasswordResetResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static SendPasswordResetResult Success() => new() { IsSuccess = true };
    public static SendPasswordResetResult Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
