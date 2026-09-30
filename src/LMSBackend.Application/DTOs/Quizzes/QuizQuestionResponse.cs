namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizQuestionResponse(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    IEnumerable<QuizAnswerOptionResponse> AnswerOptions
);