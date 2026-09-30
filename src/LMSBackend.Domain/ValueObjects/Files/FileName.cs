using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.ValueObjects.Files;

public record class FileName
{
    public string Value { get; }

    public FileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("File name cannot be empty.");

        Value = value.Trim();
    }

    public override string ToString() => Value;
}