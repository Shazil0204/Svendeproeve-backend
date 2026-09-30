namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizAnswerResponse(
    Guid Id,
    Guid QuizQuestionId,
    Guid SelectedAnswerOptionId
);