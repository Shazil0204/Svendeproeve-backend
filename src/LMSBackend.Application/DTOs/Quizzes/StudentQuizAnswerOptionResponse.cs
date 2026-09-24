namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record StudentQuizAnswerOptionResponse(
    Guid Id,
    string AnswerText
);