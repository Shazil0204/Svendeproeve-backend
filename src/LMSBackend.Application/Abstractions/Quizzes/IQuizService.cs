using LMSBackend.Application.DTOs.Quizzes;

namespace LMSBackend.Application.Abstractions.Quizzes;

public interface IQuizService
{
    Task CreateQuizAsync(CreateQuizRequest request);

    Task UpdateQuizAsync(Guid quizId, UpdateQuizRequest request);

    Task<QuizResponse> GetQuizByIdAsync(Guid quizId);

    Task<IEnumerable<QuizResponse>> GetQuizzesByUserIdAsync(Guid userId);

    Task<IEnumerable<QuizResponse>> GetAllQuizzesAsync();

    Task DeleteQuizAsync(Guid quizId);
}