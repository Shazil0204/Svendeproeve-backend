namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record TeacherDashboardAssignmentDto(Guid AssignmentId, Guid TaskId, string Title,
    Guid SubjectId, string RecipientType, Guid RecipientId, string RecipientName,
    string Status, DateTimeOffset AssignedAt, DateTimeOffset? Deadline);
