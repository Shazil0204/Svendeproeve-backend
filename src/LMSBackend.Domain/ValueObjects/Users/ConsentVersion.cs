namespace LMSBackend.Domain.ValueObjects.Users;

public record class ConsentVersion
{
    public string Value { get; }

    public ConsentVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Consent version cannot be empty.",
                nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}