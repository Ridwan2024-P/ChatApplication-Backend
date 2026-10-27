using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatWave.Business.DTOs.Auth;
using ChatWave.Business.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChatWave.Business.Services;

public class JwtTokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"]
                  ?? throw new InvalidOperationException("JWT Key is missing.");
        _issuer = configuration["Jwt:Issuer"]
                  ?? throw new InvalidOperationException("JWT Issuer is missing.");
        _audience = configuration["Jwt:Audience"]
                    ?? throw new InvalidOperationException("JWT Audience is missing.");

        _expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var m) && m > 0
            ? m
            : 10;

        var keyBytes = Convert.FromBase64String(key);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("JWT Key must contain at least 32 random bytes.");

        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    public AccessTokenDto CreateAccessToken(int userId, string phoneNumber)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.MobilePhone, phoneNumber),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        return new AccessTokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = expires
        };
    }
}
