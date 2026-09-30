using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Tasks;

namespace LMSBackend.Domain.Entities.Tasks;

public class TaskStudent
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid StudentId { get; private set; }
    public StudentTaskStatus Status { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public Task Task { get; private set; } = null!;
    public User Student { get; private set; } = null!;

    private TaskStudent() { }

    public TaskStudent(Guid taskId, Guid studentId)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        StudentId = studentId;
        Status = StudentTaskStatus.NotSubmitted;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateStatus(StudentTaskStatus newStatus)
    {
        Status = newStatus;
    }
}