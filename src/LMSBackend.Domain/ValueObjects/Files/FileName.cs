namespace LMSBackend.Domain.ValueObjects.Files;

public record class FileName
{
    public string Value { get; }

    public FileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "File name cannot be empty.",
                nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}