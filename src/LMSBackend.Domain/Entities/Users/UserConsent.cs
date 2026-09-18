using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;

namespace LMSBackend.Domain.Entities.Users
{
    public class UserConsent
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public ConsentType ConsentType { get; private set; }
        public ConsentVersion ConsentVersion { get; private set; } = null!;
        public DateTime GrantedAt { get; private set; }

        public User User { get; private set; } = null!;

        private UserConsent() { }

        public UserConsent(Guid userId, ConsentType consentType, ConsentVersion consentVersion)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ConsentType = consentType;
            ConsentVersion = consentVersion;
            GrantedAt = DateTime.UtcNow;
        }
    }
}