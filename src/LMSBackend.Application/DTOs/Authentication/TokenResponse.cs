namespace LMSBackend.Application.DTOs.Authentication;

public sealed record TokenResponse
(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt
);
