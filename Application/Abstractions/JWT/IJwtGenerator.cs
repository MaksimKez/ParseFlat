using Domain.Entities;

namespace Application.Abstractions.JWT;

public interface IJwtGenerator
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
}