namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record CreateQuizQuestionRequest(
    string QuestionText,
    IEnumerable<CreateQuizAnswerOptionRequest> AnswerOptions
);