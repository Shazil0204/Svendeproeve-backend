namespace LMSBackend.Domain.ValueObjects.Progression;

public record class Percentage
{
    public decimal Value { get; }

    public Percentage(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Percentage must be between 0 and 100.");

        Value = value;
    }

    public override string ToString() => $"{Value}%";
}