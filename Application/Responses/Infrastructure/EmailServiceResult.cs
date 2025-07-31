namespace Application.Responses.Infrastructure;

public class EmailServiceResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string ToMail { get; set; }
    
    public static EmailServiceResult Success(string toMail)
        => new() { IsSuccess = true, ToMail = toMail };
    public static EmailServiceResult Failure(string errorMessage, string toMail)
        => new() { IsSuccess = false, ErrorMessage = errorMessage, ToMail = toMail };
}