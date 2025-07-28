using Domain.Entities;

namespace Application.Common.Abstractions;

public interface IJwtGenerator
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
}