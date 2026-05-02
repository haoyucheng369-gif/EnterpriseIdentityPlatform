using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateUserToken(string username, string role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
            new("scope", "content.read"),
            new("token_type", "user")
        };

        return GenerateToken(claims);
    }

    public string GenerateServiceToken(string clientId, string scope)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, clientId),
            new("client_id", clientId),
            new("scope", scope),
            new("token_type", "service")
        };

        return GenerateToken(claims);
    }

    private string GenerateToken(List<Claim> claims)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "auth-flow-lab";
        var audience = _configuration["Jwt:Audience"] ?? "api-server";
        var privateKeyPath = _configuration["Jwt:PrivateKeyPath"]
            ?? throw new InvalidOperationException("Private key path is missing.");

        var privateKey = File.ReadAllText(privateKeyPath);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        var signingKey = new RsaSecurityKey(rsa);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}