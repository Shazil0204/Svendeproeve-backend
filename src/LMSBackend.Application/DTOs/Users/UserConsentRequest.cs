namespace LMSBackend.Application.DTOs.Users;

public sealed record UserConsentRequest
(
    bool PrivacyPolicyAccepted,
    bool TermsOfServiceAccepted
);


