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

    public async Task AddQuiz(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public async Task<Quiz?> GetQuizById(Guid quizId)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q =>
                q.Id == quizId &&
                !q.IsSoftDeleted);
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizzes()
    {
        return await _context.Quizzes
            .Where(q => !q.IsSoftDeleted)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByUserId(Guid userId)
    {
        return await _context.Quizzes
            .Where(q =>
                q.CreatedByUserId == userId &&
                !q.IsSoftDeleted)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }
}