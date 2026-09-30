using LMSBackend.Application.DTOs.Users;

namespace LMSBackend.Application.Abstractions.Users;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    Task<UserResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateUserNameAndEmailAsync(Guid userId, UpdateUserNameAndEmailRequest request, CancellationToken cancellationToken = default);

    Task SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
