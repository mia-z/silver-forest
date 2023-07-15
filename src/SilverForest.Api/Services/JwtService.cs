using Microsoft.IdentityModel.Tokens;
using SilverForest.Api.Abstraction.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SilverForest.Api.Services;

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> _logger;
    private readonly SigningCredentials credentials;
    private readonly string audience;
    private readonly string issuer;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _logger = logger;

        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            audience = configuration["Jwt:Audience"];
            issuer = configuration["Jwt:Issuer"];
        } 
        catch (Exception ex)
        {
            _logger.LogError("Exception raised while starting JwtService\n" + ex.Message);
        }
    }

    public string? GenerateJsonWebToken(int id)
    {
        _logger.LogInformation("Generating new token");

        var claims = new[]
        {
            new Claim("Id", id.ToString())
        };

        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
