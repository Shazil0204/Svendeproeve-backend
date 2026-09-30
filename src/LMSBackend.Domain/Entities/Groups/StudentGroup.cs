namespace LMSBackend.Domain.Entities.Groups;

public class StudentGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsSoftDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private StudentGroup() { }

    public StudentGroup(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void SoftDelete()
    {
        IsSoftDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}