namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record QuizReviewResponse(
    Guid QuizId,
    string QuizTitle,
    Guid StudentId,
    decimal ScorePercentage,
    DateTimeOffset? CompletedAt,
    IEnumerable<QuizReviewQuestionResponse> Questions
);