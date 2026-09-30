namespace LMSBackend.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
    void SetUserId(Guid? userId);
}