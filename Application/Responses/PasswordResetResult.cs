
namespace Application.Responses;

public class PasswordResetResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static PasswordResetResult Success() => new() { IsSuccess = true };
    public static PasswordResetResult Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
