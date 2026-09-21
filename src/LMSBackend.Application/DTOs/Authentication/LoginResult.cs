using LMSBackend.Application.DTOs.Users;
using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.DTOs.Authentication;

public sealed record LoginResult(
    TokenResponse TokenResponse,
    UserResponse User);