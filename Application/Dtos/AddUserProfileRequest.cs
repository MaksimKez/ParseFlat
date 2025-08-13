namespace Application.Dtos;

public class AddUserProfileRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PreferredLanguage { get; set; } = "EN";
}