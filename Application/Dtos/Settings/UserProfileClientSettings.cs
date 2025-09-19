namespace Application.Dtos.Settings;


public class UserProfileClientSettings
{
    public const string SectionName = "UserProfileClientSettings";
    
    public string BaseUrl { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 3;
    public double RetryDelaySeconds { get; set; } = 2;
    
    public int TimeoutSeconds { get; set; } = 30;
    public int CircuitBreakerFailureThreshold { get; set; } = 5;
    public int CircuitBreakerBreakDurationSeconds { get; set; } = 30;
    public bool UseJitter { get; set; } = true;
}
