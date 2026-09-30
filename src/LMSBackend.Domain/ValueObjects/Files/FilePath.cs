using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.ValueObjects.Files;

public record class FilePath
{
    public string Value { get; }

    public FilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("File path cannot be empty.");

        Value = value.Trim();
    }

    public override string ToString() => Value;
}