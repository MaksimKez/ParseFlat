namespace Application.Responses;

public interface IResponse
{
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
}