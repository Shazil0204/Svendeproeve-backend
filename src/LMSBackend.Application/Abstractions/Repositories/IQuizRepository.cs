using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IQuizRepository
{
    Task AddQuiz(Quiz quiz);

    Task<Quiz?> GetQuizById(Guid quizId);

    Task<IEnumerable<Quiz>> GetAllQuizzes();

    Task<IEnumerable<Quiz>> GetQuizzesByUserId(Guid userId);
}