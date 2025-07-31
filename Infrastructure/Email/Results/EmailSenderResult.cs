using Application.Responses;

namespace Infrastructure.Email.Results;

public class EmailSenderResult : IResult
{
    public int Tries { get; set; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public EmailSenderResult(bool isSuccess, string? errorMessage, int tries = 0)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public EmailSenderResult()
    { }

    public static EmailSenderResult Success()
    {
        return new EmailSenderResult(true, null);
    }

    public static EmailSenderResult Failure(string? errorMessage, int tries = 0)
    {
        return new EmailSenderResult(false, errorMessage, tries);
    }

}