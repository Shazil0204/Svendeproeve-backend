namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record CreateQuizAnswerOptionRequest(
    string AnswerText,
    bool IsCorrect
);