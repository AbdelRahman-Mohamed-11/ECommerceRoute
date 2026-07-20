using ECommerce.UseCases.Common;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Infrastructure.Identity;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> settings)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _settings = settings.Value;
    
    public AccessTokenResult GenerateToken(Guid userId, string email, string? displayName,
        IEnumerable<string> roles)
    {
        var expriresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Exp, expriresAt.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        if (!string.IsNullOrWhiteSpace(displayName))
            claims.Add(new Claim("display_name", displayName));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires : expriresAt.UtcDateTime,
                signingCredentials: credentials
            );


        var written = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(written, expriresAt);
    }
}
