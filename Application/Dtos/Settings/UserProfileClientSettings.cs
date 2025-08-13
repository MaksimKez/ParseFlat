namespace Application.Dtos.Settings;


public class UserProfileClientSettings
{
    public const string SectionName = "UserProfileClientSettings";
    public string BaseUrl { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 3;
    public double RetryDelaySeconds { get; set; } = 2;
}