using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(
            user => user.Email == email,
            cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(user => user.Role != UserRole.Administrator && !user.IsSoftDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == email && !user.IsSoftDeleted,
            cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(
            user => user.Id == id && !user.IsSoftDeleted,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ConsentType>> GetMissingConsentsAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        List<ConsentType> existingConsents = await _dbContext.UserConsents
            .Where(userConsent => userConsent.UserId == userId)
            .Select(userConsent => userConsent.ConsentType)
            .ToListAsync(cancellationToken);

        ConsentType[] requiredConsents =
        [
            ConsentType.PrivacyPolicy,
        ConsentType.TermsOfService
        ];

        List<ConsentType> missingConsents = requiredConsents
            .Where(consent => !existingConsents.Contains(consent))
            .ToList();

        return missingConsents;
    }
}
