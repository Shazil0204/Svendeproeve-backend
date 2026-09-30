using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.Exceptions;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Domain.Entities.Users;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.Student;
    public bool IsActive { get; private set; } = true;
    public bool IsSoftDeleted { get; private set; } = false;
    public DateTimeOffset? DeletedAt { get; private set; } = null;
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { } // For EF Core

    public User(string name, Email email, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UserStatus(bool isActive)
    {
        IsActive = isActive;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainValidationException("Password hash cannot be empty.");

        PasswordHash = newPasswordHash;
    }

    public void UpdateUserNameAndEmail(string newName, Email newEmail)
    {
        Name = newName?.Trim() ?? throw new DomainValidationException("Name cannot be null.");
        Email = newEmail ?? throw new DomainValidationException("Email cannot be null.");
    }

    public void ChangeUserActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }

    public void SoftDelete()
    {
        if (IsSoftDeleted)
            throw new DomainValidationException("User is already soft deleted.");
        
        Name = $"Deleted User {Id}";
        Email = new Email($"deleted+{Id}@example.com");
        IsActive = false;
        IsSoftDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
