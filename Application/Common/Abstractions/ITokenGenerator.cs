namespace Application.Common.Abstractions;

public interface ITokenGenerator
{
    string GenerateToken(int size = 32);
}