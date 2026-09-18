using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Domain.Entities.Subjects;

public class Subject
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsSoftDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public User User { get; private set; } = null!;
    private Subject() { }

    public Subject(string name, Guid createdBy)
    {
        Id = Guid.NewGuid();
        CreatedBy = createdBy;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void SoftDelete()
    {
        IsSoftDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}