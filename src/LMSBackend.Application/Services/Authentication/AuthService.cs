using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Authentication;
using LMSBackend.Application.DTOs.Users;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Auditing;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Application.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHashing _passwordHashing;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;
    public AuthService(
        IUserRepository userRepository,
        IAuthRepository authRepository,
        IPasswordHashing passwordHashing,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IAuditService auditService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _passwordHashing = passwordHashing;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }
    public async Task RegisterUserAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        Email email = new(request.Email);

        bool emailExists = await _userRepository.EmailExistsAsync(
            email,
            cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        string passwordHash =
            _passwordHashing.HashPassword(request.Password);

        User user = new(
            request.Name,
            email,
            passwordHash,
            request.Role);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoginResult> LoginUserAsync(
    LoginRequest request,
    CancellationToken cancellationToken = default)
    {
        Email email = new(request.Email);

        User? user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken) ?? throw new UnauthorizedAccessException(
                "Invalid email or password.");

        bool passwordIsValid = _passwordHashing.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsActive || user.IsSoftDeleted)
        {
            throw new UnauthorizedAccessException(
                "User account is not active.");
        }

        _currentUserService.SetUserId(user.Id);

        RefreshToken? existingRefreshToken = await _authRepository.GetActiveRefreshTokenByUserIdAsync(
            user.Id,
            cancellationToken);

        existingRefreshToken?.Revoke();

        IReadOnlyList<ConsentType> missingConsents = await _userRepository.GetMissingConsentsAsync(
            user.Id,
            cancellationToken);

        bool hasMissingConsents = missingConsents.Count > 0;

        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken();

        DateTimeOffset accessTokenExpiresAt = _tokenService.GetAccessTokenExpiration();
        DateTimeOffset refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();

        await _authRepository.AddRefreshTokenAsync(
            new RefreshToken(
                user.Id,
                refreshToken,
                refreshTokenExpiresAt),
            cancellationToken);

        await _auditService.LogAsync(
            user.Id,
            AuditAction.LoggedIn,
            "User logged in",
            nameof(User),
            user.Id,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        UserResponse userResponse = new(
            user.Id,
            user.Name,
            user.Email.Value,
            user.Role,
            user.CreatedAt,
            hasMissingConsents);

        TokenResponse tokenResponse = new(
            accessToken,
            refreshToken,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);

        return new LoginResult(
            tokenResponse,
            userResponse);
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken = default)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found.");

        RefreshToken? existingRefreshToken = await _authRepository.GetActiveRefreshTokenByUserIdAsync(
            user.Id,
            cancellationToken);

        existingRefreshToken?.Revoke();

        await _auditService.LogAsync(
            user.Id,
            AuditAction.RefreshTokenRevoked,
            "User refresh token revoked",
            nameof(User),
            user.Id,
            cancellationToken);

        await _auditService.LogAsync(
            user.Id,
            AuditAction.LoggedOut,
            "User logged out",
            nameof(User),
            user.Id,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddUserConsentAsync(
        UserConsentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.PrivacyPolicyAccepted)
        {
            throw new ValidationException("PrivacyPolicy must be Accepted.");
        }

        if (!request.TermsOfServiceAccepted)
        {
            throw new ValidationException("TermsOfService must be Accepted.");
        }

        User? user = await _userRepository.GetByEmailAsync(
            new Email(request.Email),
            cancellationToken) ?? throw new NotFoundException("User not found.");

        UserConsent userConsentForTermOfService = new(
            user.Id,
            ConsentType.TermsOfService
        );

        UserConsent userConsentForPrivacyPolicy = new(
            user.Id,
            ConsentType.PrivacyPolicy
        );

        await _authRepository.AddUserConsentAsync(userConsentForTermOfService, cancellationToken);
        await _authRepository.AddUserConsentAsync(userConsentForPrivacyPolicy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TokenResponse> RenewRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? existingRefreshToken = await _authRepository.GetRefreshTokenByHashAsync(
            refreshToken,
            cancellationToken);

        if (existingRefreshToken is null || existingRefreshToken.IsRevoked || existingRefreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        User? user = await _userRepository.GetByIdAsync(
            existingRefreshToken.UserId,
            cancellationToken) ?? throw new UnauthorizedAccessException("User not found.");

        existingRefreshToken.Revoke();

        string newAccessToken = _tokenService.GenerateAccessToken(user);
        string newRefreshToken = _tokenService.GenerateRefreshToken();

        DateTimeOffset newAccessTokenExpiresAt = _tokenService.GetAccessTokenExpiration();
        DateTimeOffset newRefreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();

        await _authRepository.AddRefreshTokenAsync(
            new RefreshToken(
                user.Id,
                newRefreshToken,
                newRefreshTokenExpiresAt),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(
            newAccessToken,
            newRefreshToken,
            newAccessTokenExpiresAt,
            newRefreshTokenExpiresAt);
    }

    public async Task UpdatePasswordAsync(
        UpdatePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByEmailAsync(
            new Email(request.Email),
            cancellationToken) ?? throw new NotFoundException("User not found.");

        user.UpdatePassword(_passwordHashing.HashPassword(request.NewPassword));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
