using LMSBackend.Application.DTOs.Quizzes;

namespace LMSBackend.Application.Abstractions.Quizzes;

public interface IQuizService
{
    Task AddQuiz(CreateQuizRequest quiz);
    Task<QuizResponse?> GetQuizById(Guid quizId);
    Task<IEnumerable<QuizResponse>> GetAllQuizzes();
    Task UpdateQuiz(Guid quizId, UpdateQuizRequest updatedQuiz);
    Task DeleteQuiz(Guid quizId);
}
