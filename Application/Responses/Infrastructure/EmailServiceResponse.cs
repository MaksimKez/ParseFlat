namespace Application.Responses.Infrastructure;

public class EmailServiceResponse : IResponse
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string ToMail { get; set; }
}