namespace Application.Responses;

public class Result : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static Result Success() => new Result { IsSuccess = true };
    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}