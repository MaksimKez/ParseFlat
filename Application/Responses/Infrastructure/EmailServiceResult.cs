namespace Application.Responses.Infrastructure;

public class EmailServiceResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string ToMail { get; set; }
}