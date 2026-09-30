namespace LMSBackend.Application.DTOs.Quizzes;

public sealed record StudentQuizResponse(
    Guid Id,
    string Title,
    string Description,
    IEnumerable<StudentQuizQuestionResponse> Questions
); 