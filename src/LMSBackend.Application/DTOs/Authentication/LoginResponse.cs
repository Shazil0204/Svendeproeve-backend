using LMSBackend.Application.DTOs.Users;
using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.DTOs.Authentication;

public sealed record LoginResponse(
    UserResponse User,
    IReadOnlyList<ConsentType> MissingConsents);