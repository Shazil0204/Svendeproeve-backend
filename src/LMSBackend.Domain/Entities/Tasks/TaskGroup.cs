using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Enums.Tasks;

namespace LMSBackend.Domain.Entities.Tasks;

public class TaskGroup
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid GroupId { get; private set; }
    public StudentTaskStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Task Task { get; private set; } = null!;
    public StudentGroup Group { get; private set; } = null!;

    private TaskGroup() { }

    public TaskGroup(Guid taskId, Guid groupId)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        GroupId = groupId;
        Status = StudentTaskStatus.NotSubmitted;
        AssignedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(StudentTaskStatus newStatus)
    {
        Status = newStatus;
    }
}