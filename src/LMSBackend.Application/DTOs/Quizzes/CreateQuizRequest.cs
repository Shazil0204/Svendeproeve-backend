namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record CreateQuizRequest(
    Guid SubjectId,
    string Title,
    string Description,
    decimal PassingPercentage,
    IEnumerable<CreateQuizQuestionRequest> Questions
);