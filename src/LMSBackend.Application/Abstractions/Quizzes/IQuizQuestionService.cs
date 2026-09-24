using LMSBackend.Application.DTOs.Quizzes;

namespace LMSBackend.Application.Abstractions.Quizzes;

public interface IQuizQuestionService
{
    Task AddQuestionAsync(
        Guid quizId,
        CreateQuizQuestionRequest request);

    Task UpdateQuestionAsync(
        Guid questionId,
        UpdateQuizQuestionRequest request);

    Task<IEnumerable<QuizQuestionResponse>>
        GetQuestionsByQuizIdAsync(Guid quizId);

    Task DeleteQuestionAsync(Guid questionId);
}