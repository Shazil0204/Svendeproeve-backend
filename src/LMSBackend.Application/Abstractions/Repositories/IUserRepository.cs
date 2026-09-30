using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConsentType>> GetMissingConsentsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<(User User, bool IsMissingConsents)>> GetAllWithConsentStatusAsync(
        CancellationToken cancellationToken = default);
}