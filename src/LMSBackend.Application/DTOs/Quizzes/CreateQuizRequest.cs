namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record CreateQuizRequest
(
    Guid SubjectId,
    string Title,
    string Description,
    int PassingPercentage
);