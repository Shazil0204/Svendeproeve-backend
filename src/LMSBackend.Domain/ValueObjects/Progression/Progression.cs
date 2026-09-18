namespace LMSBackend.Domain.ValueObjects.Progression;

public record class Progression
{
    public int Completed { get; }

    public int Total { get; }

    public Percentage Percentage { get; }

    public Progression(int completed, int total)
    {
        if (completed < 0)
            throw new ArgumentOutOfRangeException(nameof(completed));

        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total));

        if (completed > total)
            throw new ArgumentException(
                "Completed activities cannot exceed total activities.");

        Completed = completed;
        Total = total;

        Percentage = new Percentage(
            total == 0
                ? 0
                : (decimal)completed / total * 100);
    }
}