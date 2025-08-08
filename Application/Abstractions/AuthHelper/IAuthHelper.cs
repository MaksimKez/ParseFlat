using Application.Responses;

namespace Application.Abstractions.AuthHelper;

public interface IAuthHelper
{
    AuthHelperResult GetRefreshToken(IEnumerable<KeyValuePair<string, string>> valuePairs);
    AuthHelperResult GetEmailFromToken(string token);
}