using LMSBackend.Application.DTOs.Authentication;
using LMSBackend.Application.DTOs.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task RegisterUserAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginUserAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task LogoutUserAsync(
        CancellationToken cancellationToken = default);

    Task AddUserConsentAsync(
        UserConsentRequest request,
        CancellationToken cancellationToken = default);

    Task<TokenResponse> RenewRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);
}
