using LMSBackend.Application.DTOs.Users;
using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.DTOs.Authentication;

public sealed record LoginResult(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    UserResponse User);