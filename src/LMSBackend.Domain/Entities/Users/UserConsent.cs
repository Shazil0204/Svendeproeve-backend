using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Domain.Entities.Users
{
    public class UserConsent
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public ConsentType ConsentType { get; private set; }
        public string ConsentVersion { get; private set; } = null!;
        public DateTimeOffset GrantedAt { get; private set; }

        public User User { get; private set; } = null!;

        private UserConsent() { }

        public UserConsent(Guid userId, ConsentType consentType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ConsentType = consentType;
            ConsentVersion = "Version 1.0.0";
            GrantedAt = DateTimeOffset.UtcNow;
        }
    }
}