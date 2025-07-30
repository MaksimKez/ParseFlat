namespace Application.Abstractions.Security;

public interface ITokenGenerator
{
    string GenerateToken(int size = 32);
}