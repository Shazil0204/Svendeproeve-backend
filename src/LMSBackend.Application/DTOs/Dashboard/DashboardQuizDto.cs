namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record DashboardQuizDto(Guid AssignmentId, Guid QuizId, string Title, Guid SubjectId,
    string Status, decimal? ScorePercentage, bool? Passed, DateTimeOffset AssignedAt,
    DateTimeOffset? CompletedAt);
