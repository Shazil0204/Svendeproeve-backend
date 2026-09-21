using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Application.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    DateTimeOffset GetRefreshTokenExpiration();
    DateTimeOffset GetAccessTokenExpiration();
}