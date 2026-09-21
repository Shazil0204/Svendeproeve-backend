using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.ValueObjects.Users;

public record class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("Email cannot be empty.");

        value = value.Trim();

        if (!value.Contains('@'))
            throw new DomainValidationException("Email is invalid.");

        Value = value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}