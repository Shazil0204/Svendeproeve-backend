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
        LoginFailed = 7,
        PasswordChanged = 8
    }
}