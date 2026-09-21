using LMSBackend.Domain.Enums.Users;

namespace LMSBackend.Application.DTOs.Authentication;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    UserRole Role);