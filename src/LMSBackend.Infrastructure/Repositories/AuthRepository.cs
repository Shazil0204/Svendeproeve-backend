using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.ValueObjects.Users;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _dbContext;

    public AuthRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegisterUserAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User> LoginUserAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task SoftDeleteUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddUserConsentAsync(
        UserConsent consent,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.UserConsents.AddAsync(consent, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                token =>
                    token.UserId == userId &&
                    token.RevokedAt == null &&
                    token.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);
    }
}