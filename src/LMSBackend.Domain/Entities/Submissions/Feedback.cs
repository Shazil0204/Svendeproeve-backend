using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Domain.Entities.Submissions;

public class Feedback
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Guid TeacherId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public Submission Submission { get; private set; } = null!;
    public User Teacher { get; private set; } = null!;

    private Feedback() { }

    public Feedback(
        Guid submissionId,
        Guid teacherId,
        string content)
    {
        Id = Guid.NewGuid();
        SubmissionId = submissionId;
        TeacherId = teacherId;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}