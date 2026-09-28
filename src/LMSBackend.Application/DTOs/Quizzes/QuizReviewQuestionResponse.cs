namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizReviewQuestionResponse(
    Guid QuestionId,
    string QuestionText,
    string? SelectedAnswerText,
    string CorrectAnswerText,
    bool IsCorrect
);