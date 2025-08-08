namespace Application.Dtos.Settings;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public CookieSettings Cookie { get; set; } = new();
    public JwtSettings Jwt { get; set; } = new();
    public MessageSettings Messages { get; set; } = new();

    public class CookieSettings
    {
        public string RefreshTokenName { get; set; } = "refreshToken";
        public int ExpirationHours { get; set; } = 7;
        public bool HttpOnly { get; set; } = true;
        public bool Secure { get; set; } = true;
    }

    public class JwtSettings
    {
        public string EmailClaimType { get; set; } = "email";
    }

    public class MessageSettings
    {
        public string PasswordChanged { get; set; } = "Password changed successfully";
        public string RefreshTokenMissing { get; set; } = "Refresh token is missing";
        public string VerificationLinkSent { get; set; } = "Verification link sent";
        public string EmailVerified { get; set; } = "Email successfully verified";
        public string VerificationTokenMissing { get; set; } = "Verification token is missing";
        public string RefreshTokenNotFound { get; set; } = "Refresh token not found in cookies";
        public string EmailNotFoundInToken { get; set; } = "Email not found in token";
        public string InvalidTokenFormat { get; set; } = "Invalid token format";
    }
}
