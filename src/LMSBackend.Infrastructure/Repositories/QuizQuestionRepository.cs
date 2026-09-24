using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class QuizQuestionRepository : IQuizQuestionRepository
{
    private readonly AppDbContext _context;

    public QuizQuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddQuizQuestion(QuizQuestion quizQuestion)
    {
        await _context.QuizQuestions.AddAsync(quizQuestion);
    }

    public async Task<QuizQuestion?> GetQuizQuestionById(Guid quizQuestionId)
    {
        return await _context.QuizQuestions
            .FirstOrDefaultAsync(q => q.Id == quizQuestionId);
    }

    public async Task<IEnumerable<QuizQuestion>> GetQuizQuestionsByQuizId(
        Guid quizId)
    {
        return await _context.QuizQuestions
            .Where(q => q.QuizId == quizId)
            .ToListAsync();
    }

    public void DeleteQuizQuestion(QuizQuestion quizQuestion)
    {
        _context.QuizQuestions.Remove(quizQuestion);
    }

    public async Task<bool> HasStudentAnswers(Guid quizQuestionId)
    {
        return await _context.QuizAnswers
            .AnyAsync(a => a.QuizQuestionId == quizQuestionId);
    }
}