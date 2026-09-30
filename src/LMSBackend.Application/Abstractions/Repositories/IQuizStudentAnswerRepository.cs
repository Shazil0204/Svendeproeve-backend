using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IQuizStudentAnswerRepository
{
    // Quiz Student

    Task AddQuizStudent(QuizStudent quizStudent);

    Task<QuizStudent?> GetQuizStudent(
        Guid quizId,
        Guid studentId);

    Task<IEnumerable<QuizStudent>> GetQuizStudentsByQuizId(
        Guid quizId);

    void DeleteQuizStudent(QuizStudent quizStudent);


    // Student Answers

    Task AddStudentQuizAnswer(QuizAnswer quizAnswer);

    Task<bool> HasAnsweredQuestion(
        Guid quizStudentId,
        Guid quizQuestionId);

    Task<IEnumerable<QuizAnswer>> GetStudentAnswersByQuizStudentId(
        Guid quizStudentId);

    Task<QuizAnswer?> GetStudentAnswerByQuestionId(
        Guid quizStudentId,
        Guid quizQuestionId);
}