namespace Application.Responses;

public class RegisterUserResult : Result
{
    public Guid RegisteredUserId { get; private init; }

    public static RegisterUserResult Success(Guid registeredUserId)
    {
        return new RegisterUserResult
        {
            RegisteredUserId = registeredUserId,
            ErrorMessage = null,
            IsSuccess = true
        };
    }

    public new static RegisterUserResult Failure(string? errorMessage)
    {
        return new RegisterUserResult
        {
            ErrorMessage = errorMessage,
            IsSuccess = false,
            RegisteredUserId = Guid.Empty
        };
    }
}
