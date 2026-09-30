using LMSBackend.Domain.Enums.Tasks;

namespace LMSBackend.Application.DTOs.Dashboard;

public sealed class TeacherAssignmentRow
{
    public Guid AssignmentId { get; init; }
    public Guid TaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid SubjectId { get; init; }
    public string RecipientType { get; init; } = string.Empty;
    public Guid RecipientId { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public StudentTaskStatus Status { get; init; }
    public DateTimeOffset AssignedAt { get; init; }
    public DateTimeOffset? Deadline { get; init; }
}
