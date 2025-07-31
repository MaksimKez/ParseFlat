namespace Application.Dtos;

public class SendVerificationEmailRequest
{
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
}