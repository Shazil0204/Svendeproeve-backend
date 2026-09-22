using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;

    public QuizRepository(AppDbContext context)
    {
        _context = context;
    }

    # region QUIZ REPOSITORY METHODS
    public async Task AddQuiz(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public async Task<Quiz?> GetQuizById(Guid quizId)
    {
        return await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId && !q.IsSoftDeleted);
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizzes()
    {
        return await _context.Quizzes
            .Where(q => !q.IsSoftDeleted)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }
    # endregion

    # region QUIZ QUESTIONS REPOSITORY METHODS

    public async Task<IEnumerable<Quiz>> GetQuizzesBySubjectId(Guid subjectId)
    {
        return await _context.Quizzes
            .Where(q => q.SubjectId == subjectId && !q.IsSoftDeleted)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task AddQuizQuestion(QuizQuestion quizQuestion)
    {
        await _context.QuizQuestions.AddAsync(quizQuestion);
    }

    public async Task<QuizQuestion?> GetQuizQuestionById(Guid quizQuestionId)
    {
        return await _context.QuizQuestions.FirstOrDefaultAsync(qq => qq.Id == quizQuestionId);
    }

    public async Task<IEnumerable<QuizQuestion>> GetQuizQuestionsByQuizId(Guid quizId)
    {
        return await _context.QuizQuestions
            .Where(qq => qq.QuizId == quizId)
            .ToListAsync();
    }

    public async Task DeleteQuizQuestion(QuizQuestion quizQuestion)
    {
        _context.QuizQuestions.Remove(quizQuestion);
    }

    # endregion

    # region QUIZ ANSWERS OPTIONS REPOSITORY METHODS

    public async Task AddQuizAnswer(QuizAnswerOption quizAnswer)
    {
        await _context.QuizAnswerOptions.AddAsync(quizAnswer);
    }

    public async Task<IEnumerable<QuizAnswerOption>> GetQuizAnswerOptionsByQuestionId(Guid quizQuestionId)
    {
        return await _context.QuizAnswerOptions
            .Where(qa => qa.QuizQuestionId == quizQuestionId)
            .ToListAsync();
    }

    public async Task DeleteQuizAnswerOption(QuizAnswerOption quizAnswerOption)
    {
        _context.QuizAnswerOptions.Remove(quizAnswerOption);
    }

    # endregion

    # region QUIZ STUDENT REPOSITORY METHODS

    public async Task AddQuizStudent(QuizStudent quizStudent)
    {
        await _context.QuizStudents.AddAsync(quizStudent);
    }

    public async Task<IEnumerable<QuizStudent>> GetQuizStudentsByQuizId(Guid quizId)
    {
        return await _context.QuizStudents
            .Where(qs => qs.QuizId == quizId)
            .ToListAsync();
    }

    public async Task DeleteQuizStudent(QuizStudent quizStudent)
    {
        _context.QuizStudents.Remove(quizStudent);
    }

    # endregion

    # region STUDENT QUIZ ANSWERS REPOSITORY METHODS

    public async Task AddStudentQuizAnswer(QuizAnswer quizAnswer)
    {
        await _context.QuizAnswers.AddAsync(quizAnswer);
    }

    public async Task<IEnumerable<QuizAnswer>> GetStudentQuizAnswersByQuizId(Guid quizId)
    {
        return await _context.QuizAnswers
            .Where(qa => qa.QuizQuestion.QuizId == quizId)
            .ToListAsync();
    }

    # endregion
}
