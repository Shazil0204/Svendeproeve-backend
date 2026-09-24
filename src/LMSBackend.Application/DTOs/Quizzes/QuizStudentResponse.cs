namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizStudentResponse(
    Guid Id,
    Guid QuizId,
    Guid StudentId,
    DateTimeOffset AssignedAt,
    decimal? ScorePercentage,
    bool? Passed,
    DateTimeOffset? CompletedAt,
    string Status
);