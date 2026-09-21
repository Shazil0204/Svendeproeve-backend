using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.DTOs.Users;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    DateTimeOffset CreatedAt,
    bool IsMissingConsents
);
