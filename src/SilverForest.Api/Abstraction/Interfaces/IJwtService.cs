namespace SilverForest.Api.Abstraction.Interfaces;

public interface IJwtService
{
    string? GenerateJsonWebToken(int id);
}
