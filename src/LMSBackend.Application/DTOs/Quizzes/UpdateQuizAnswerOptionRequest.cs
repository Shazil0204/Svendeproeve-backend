namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record UpdateQuizAnswerOptionRequest(
    string AnswerText,
    bool IsCorrect
);