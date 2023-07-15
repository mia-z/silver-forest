namespace SilverForest.Api.Abstraction;

public interface IJwtService
{
    string? GenerateJsonWebToken(int id);
}
