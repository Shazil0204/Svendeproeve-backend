using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Domain.Entities.Tasks;

public class Task
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime? Deadline { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsSoftDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Subject Subject { get; private set; } = null!;
    public User CreatedByUser { get; private set; } = null!;

    private Task() { }

    public Task(Guid subjectId, Guid createdByUserId, string title, string description, DateTime? deadline)
    {
        Id = Guid.NewGuid();
        SubjectId = subjectId;
        CreatedByUserId = createdByUserId;
        Title = title;
        Description = description;
        Deadline = deadline;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, DateTime? deadline)
    {
        Title = title;
        Description = description;
        Deadline = deadline;
    }

    public void SoftDelete()
    {
        IsSoftDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}