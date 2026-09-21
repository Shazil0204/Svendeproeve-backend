namespace LMSBackend.Application.DTOs.Users;

public sealed record UpdateUserNameAndEmailRequest(
    string? Name,
    string? Email);
