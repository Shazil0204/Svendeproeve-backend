namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizResponse(
    Guid Id,
    Guid SubjectId,
    Guid CreatedByUserId,
    string Title,
    string Description,
    decimal PassingPercentage,
    DateTimeOffset CreatedAt
);