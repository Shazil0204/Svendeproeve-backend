using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.ValueObjects.Progression;

namespace LMSBackend.Domain.Entities.Quizzes;

public class Quiz
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Percentage PassingPercentage { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsSoftDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Subject Subject { get; private set; } = null!;
    public User CreatedByUser { get; private set; } = null!;

    private Quiz() { }

    public Quiz(
        Guid subjectId,
        Guid createdByUserId,
        string title,
        string description,
        Percentage passingPercentage)
    {
        Id = Guid.NewGuid();
        SubjectId = subjectId;
        CreatedByUserId = createdByUserId;
        Title = title;
        Description = description;
        PassingPercentage = passingPercentage;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(
        string title,
        string description,
        Percentage passingPercentage)
    {
        Title = title;
        Description = description;
        PassingPercentage = passingPercentage;
    }

    public void SoftDelete()
    {
        if (IsSoftDeleted)
            throw new InvalidOperationException("Quiz is already soft deleted.");

        IsSoftDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}