using LMSBackend.Domain.Entities.Tasks;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.ValueObjects.Tasks;

namespace LMSBackend.Domain.Entities.Submissions;

public class Submission
{
    public Guid Id { get; private set; }
    public Guid? TaskStudentId { get; private set; }
    public Guid? TaskGroupId { get; private set; }
    public Guid SubmittedByUserId { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset SubmittedAt { get; private set; }
    public TaskStudent? TaskStudent { get; private set; }
    public TaskGroup? TaskGroup { get; private set; }
    public User SubmittedByUser { get; private set; } = null!;

    private Submission() { }

    public Submission(
        AssignmentTarget assignmentTarget,
        Guid submittedByUserId,
        string? comment)
    {
        Id = Guid.NewGuid();

        TaskStudentId = assignmentTarget.TaskStudentId;
        TaskGroupId = assignmentTarget.TaskGroupId;

        SubmittedByUserId = submittedByUserId;
        Comment = comment;
        SubmittedAt = DateTimeOffset.UtcNow;
    }
}