namespace LMSBackend.Domain.ValueObjects.Files;

public record class FilePath
{
    public string Value { get; }

    public FilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "File path cannot be empty.",
                nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}