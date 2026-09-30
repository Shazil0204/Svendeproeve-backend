namespace LMSBackend.Domain.Enums.Auditing
{
    public enum AuditAction
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Assigned = 4,
        LoggedIn = 5,
        LoggedOut = 6,
        PasswordChanged = 7,
        RefreshTokenRevoked = 8
    }
}