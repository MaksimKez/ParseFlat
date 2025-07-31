namespace Application.Responses;

public class SendVerificationEmailResult : IResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ToEmail { get; set; }
    
    public static SendVerificationEmailResult Success(string toEmail)
        => new SendVerificationEmailResult { IsSuccess = true, ToEmail = toEmail };
    public static SendVerificationEmailResult Failed(string errorMessage, string toEmail) 
        => new SendVerificationEmailResult { IsSuccess = false, ErrorMessage = errorMessage, ToEmail = toEmail };
}