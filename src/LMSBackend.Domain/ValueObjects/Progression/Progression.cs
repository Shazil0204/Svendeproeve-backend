using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.ValueObjects.Progression;

public record class Progression
{
    public int Completed { get; }

    public int Total { get; }

    public Percentage Percentage { get; }

    public Progression(int completed, int total)
    {
        if (completed < 0)
            throw new DomainValidationException("Completed activities cannot be negative.");

        if (total < 0)
            throw new DomainValidationException("Total activities cannot be negative.");

        if (completed > total)
            throw new DomainValidationException("Completed activities cannot exceed total activities.");

        Completed = completed;
        Total = total;

        Percentage = new Percentage(
            total == 0
                ? 0
                : (decimal)completed / total * 100);
    }
}