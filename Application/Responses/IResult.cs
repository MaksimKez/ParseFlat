namespace Application.Responses;

public interface IResult
{
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
}