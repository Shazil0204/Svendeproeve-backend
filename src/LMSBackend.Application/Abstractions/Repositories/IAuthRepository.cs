using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IAuthRepository
{
    // Consent
    Task AddUserConsentAsync(
        UserConsent consent,
        CancellationToken cancellationToken = default);

    // Refresh Token
    Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetActiveRefreshTokenByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}