using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    private string GenerateGenericStringTokenWithClaims(Claim[] claims)
    {
        var signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256);

        // Generate the token
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddHours(1),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRegistrationToken(string email)
    {
        // Create claims for the token
        var claims = new[]
        {
            new Claim("action", "register"),
            new Claim("email", email)
        };

        return GenerateGenericStringTokenWithClaims(claims);
    }

    public string GenerateLoginToken(string username, string firstName, string lastName, string email)
    {
        // Create claims for the token
        var claims = new[]
        {
            new Claim("action", "login"),
            new Claim("username", username),
            new Claim("firstName", firstName),
            new Claim("lastName", lastName),
            new Claim("email", email)
        };

        return GenerateGenericStringTokenWithClaims(claims);
    }

    public string GeneratePasswordResetToken(Guid id)
    {
        // Create claims for the token
        var claims = new[]
        {
            new Claim("action", "resetPassword"),
            new Claim("userid", id.ToString())
        };

        return GenerateGenericStringTokenWithClaims(claims);
    }
}