using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IQuizQuestionRepository
{
    Task AddQuizQuestion(QuizQuestion quizQuestion);

    Task<QuizQuestion?> GetQuizQuestionById(Guid quizQuestionId);

    Task<IEnumerable<QuizQuestion>> GetQuizQuestionsByQuizId(Guid quizId);

    void DeleteQuizQuestion(QuizQuestion quizQuestion);

    Task<bool> HasStudentAnswers(Guid quizQuestionId);
}