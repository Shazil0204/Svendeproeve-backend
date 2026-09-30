namespace LMSBackend.Application.DTOs.Submissions;

public sealed record FeedbackDto(Guid Id, Guid StudentId, Guid TaskId, string Text,
    DateTimeOffset CreatedAt, Guid TeacherId, string TeacherName, Guid SubmissionId, Guid AssignmentId);
