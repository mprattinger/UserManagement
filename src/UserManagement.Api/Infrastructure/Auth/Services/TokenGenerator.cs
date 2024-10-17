using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Api.Features.Auth.Models;

namespace UserManagement.Api.Infrastructure.Auth.Services;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}

public class TokenGenerator(IDateTimeProvider dateTimeProvider, JwtSettings jwtSettings) : ITokenGenerator
{
    public string GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret ?? "")),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim("UMUID", user.Id.ToString())
        };

        var secToken = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: dateTimeProvider.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes ?? 0));

        return new JwtSecurityTokenHandler().WriteToken(secToken);
    }
}
