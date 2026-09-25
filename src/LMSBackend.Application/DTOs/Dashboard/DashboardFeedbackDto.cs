namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record DashboardFeedbackDto(Guid Id, Guid TaskId, string TaskTitle, Guid AssignmentId,
    Guid SubmissionId, string Text, DateTimeOffset CreatedAt, Guid TeacherId, string TeacherName);
