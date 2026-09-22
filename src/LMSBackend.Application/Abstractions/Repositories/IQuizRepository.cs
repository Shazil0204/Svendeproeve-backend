using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IQuizRepository
{
    Task AddQuiz(Quiz quiz);
    Task<Quiz?> GetQuizById(Guid quizId);
    Task<IEnumerable<Quiz>> GetAllQuizzes();
    Task<IEnumerable<Quiz>> GetQuizzesBySubjectId(Guid subjectId);
    Task AddQuizQuestion(QuizQuestion quizQuestion);
    Task<QuizQuestion?> GetQuizQuestionById(Guid quizQuestionId);
    Task<IEnumerable<QuizQuestion>> GetQuizQuestionsByQuizId(Guid quizId);
    Task DeleteQuizQuestion(QuizQuestion quizQuestion);
    Task AddQuizAnswer(QuizAnswerOption quizAnswer);
    Task<IEnumerable<QuizAnswerOption>> GetQuizAnswerOptionsByQuestionId(Guid quizQuestionId);
    Task DeleteQuizAnswerOption(QuizAnswerOption quizAnswerOption);
    Task AddQuizStudent(QuizStudent quizStudent);
    Task<IEnumerable<QuizStudent>> GetQuizStudentsByQuizId(Guid quizId);
    Task DeleteQuizStudent(QuizStudent quizStudent);
    Task AddStudentQuizAnswer(QuizAnswer quizAnswer);
    Task<IEnumerable<QuizAnswer>> GetStudentQuizAnswersByQuizQuestionId(Guid quizQuestionId);
}
