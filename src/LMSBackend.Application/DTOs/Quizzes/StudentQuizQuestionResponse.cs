namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record StudentQuizQuestionResponse(
    Guid Id,
    string QuestionText,
    IEnumerable<StudentQuizAnswerOptionResponse> AnswerOptions,
    Guid? SelectedAnswerOptionId
);