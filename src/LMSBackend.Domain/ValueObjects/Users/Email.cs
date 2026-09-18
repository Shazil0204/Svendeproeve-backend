namespace LMSBackend.Domain.ValueObjects.Users;

public record class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        value = value.Trim();

        if (!value.Contains('@'))
            throw new ArgumentException("Email is invalid.", nameof(value));

        Value = value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}