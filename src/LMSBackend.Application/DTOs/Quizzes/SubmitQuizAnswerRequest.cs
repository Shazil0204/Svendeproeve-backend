namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record SubmitQuizAnswerRequest(
    Guid QuizQuestionId,
    Guid SelectedAnswerOptionId
);