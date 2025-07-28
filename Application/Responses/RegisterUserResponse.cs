namespace Application.Responses;

public class RegisterUserResponse : IResponse
{
    public Guid RegisteredUserId { get; set; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public RegisterUserResponse(Guid registeredUserId, bool isSuccess, string? errorMessage)
    {
        RegisteredUserId = registeredUserId;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public RegisterUserResponse()
    { }

    public static RegisterUserResponse Success(Guid registeredUserId)
    {
        return new RegisterUserResponse(registeredUserId, true, null);
    }

    public static RegisterUserResponse Failure(string? errorMessage)
    {
        return new RegisterUserResponse(Guid.Empty, false, errorMessage);
    }
}