namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record DashboardTaskDto(Guid AssignmentId, Guid TaskId, string Title, Guid SubjectId,
    string RecipientType, Guid? GroupId, string Status, DateTimeOffset AssignedAt,
    DateTimeOffset? Deadline, DateTimeOffset? SubmittedAt, int FeedbackCount);
