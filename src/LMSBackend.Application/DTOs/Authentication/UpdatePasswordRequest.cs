namespace LMSBackend.Application.DTOs.Authentication;

public sealed record UpdatePasswordRequest(
    string Email,
    string NewPassword
);