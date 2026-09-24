namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizAnswerOptionResponse(
    Guid Id,
    Guid QuizQuestionId,
    string AnswerText,
    bool IsCorrect
);