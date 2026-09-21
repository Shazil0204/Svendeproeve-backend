namespace LMSBackend.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    void SetUserId(Guid? userId);
}