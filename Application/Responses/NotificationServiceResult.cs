namespace Application.Responses;

public class NotificationServiceResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}