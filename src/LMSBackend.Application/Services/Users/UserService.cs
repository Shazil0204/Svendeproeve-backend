using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Users;
using LMSBackend.Application.DTOs.Users;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditLogService;
    private readonly IAuthRepository _authRepository;
    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAuditService auditLogService, IAuthRepository authRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _authRepository = authRepository;
    }

    public async Task<UserResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt,
            false // If User is Logged in then he has given all the consents
        );
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt,
            false // If User is Logged in then he has given all the consents
        );
    }

    public async Task<UserResponse> UpdateUserNameAndEmailAsync(Guid userId, UpdateUserNameAndEmailRequest request, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");

        string name = request.Name ?? user.Name;
        Email email = new(request.Email ?? user.Email.Value);

        user.UpdateUserNameAndEmail(name, email);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt,
            false // If User is Logged in then he has given all the consents
        );
    }

    public async Task SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");

        RefreshToken? refreshToken = await _authRepository.GetActiveRefreshTokenByUserIdAsync(userId, cancellationToken);

        refreshToken?.Revoke();

        user.SoftDelete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeUserActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");

        user.ChangeUserActiveStatus(isActive);

        if (!isActive)
        {
            RefreshToken? refreshToken = await _authRepository.GetActiveRefreshTokenByUserIdAsync(userId, cancellationToken);

            refreshToken?.Revoke();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}