using System.Security.Claims;
using LMSBackend.Application.Abstractions.Authentication;

namespace LMSBackend.API.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _explicitUserId;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            if (_explicitUserId.HasValue)
            {
                return _explicitUserId;
            }

            string? value = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Role =>
        _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Role);

    public void SetUserId(Guid? userId)
    {
        _explicitUserId = userId;
    }
}