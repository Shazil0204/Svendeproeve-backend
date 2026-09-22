namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record UpdateQuizRequest
(
    string? Title,
    string? Description,
    int? PassingPercentage
);