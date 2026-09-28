namespace LMSBackend.Application.DTOs.Users;

public sealed record UserConsentRequest
(
    string Email,
    bool PrivacyPolicyAccepted,
    bool TermsOfServiceAccepted
);


