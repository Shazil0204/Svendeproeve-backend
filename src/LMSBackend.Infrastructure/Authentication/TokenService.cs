using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Domain.Entities.Users;
using Microsoft.IdentityModel.Tokens;

namespace LMSBackend.Infrastructure.Authentication;

public sealed class TokenService : ITokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public TokenService()
    {
        _secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new InvalidOperationException(
                "JWT_SECRET is not configured.");

        _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? throw new InvalidOperationException(
                "JWT_ISSUER is not configured.");

        _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? throw new InvalidOperationException(
                "JWT_AUDIENCE is not configured.");

        string accessTokenMinutes =
            Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES")
            ?? throw new InvalidOperationException(
                "JWT_ACCESS_TOKEN_MINUTES is not configured.");

        string refreshTokenDays =
            Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DAYS")
            ?? throw new InvalidOperationException(
                "JWT_REFRESH_TOKEN_DAYS is not configured.");

        _accessTokenMinutes = int.Parse(accessTokenMinutes);
        _refreshTokenDays = int.Parse(refreshTokenDays);
    }

    public string GenerateAccessToken(User user)
    {
        Claim[] claims =
        [
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        ];

        SymmetricSecurityKey key = new(
            Encoding.UTF8.GetBytes(_secret));

        SigningCredentials credentials = new(
            key,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public string HashRefreshToken(string refreshToken)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        byte[] hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }

    public DateTimeOffset GetAccessTokenExpiration()
    {
        return DateTimeOffset.UtcNow.AddMinutes(_accessTokenMinutes);
    }

    public DateTimeOffset GetRefreshTokenExpiration()
    {
        return DateTimeOffset.UtcNow.AddDays(_refreshTokenDays);
    }
}