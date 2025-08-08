namespace Application.Responses;

public class RegisterUserResult : IResult
{
    public Guid RegisteredUserId { get; set; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public RegisterUserResult(Guid registeredUserId, bool isSuccess, string? errorMessage)
    {
        RegisteredUserId = registeredUserId;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public RegisterUserResult()
    { }

    public static RegisterUserResult Success(Guid registeredUserId)
    {
        return new RegisterUserResult(registeredUserId, true, null);
    }

    public static RegisterUserResult Failure(string? errorMessage)
    {
        return new RegisterUserResult(Guid.Empty, false, errorMessage);
    }
}