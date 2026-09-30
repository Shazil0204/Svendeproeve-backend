using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class QuizAnswerOptionRepository : IQuizAnswerOptionRepository
{
    private readonly AppDbContext _context;

    public QuizAnswerOptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddQuizAnswerOption(QuizAnswerOption answerOption)
    {
        await _context.QuizAnswerOptions.AddAsync(answerOption);
    }

    public async Task<QuizAnswerOption?> GetQuizAnswerOptionById(
        Guid answerOptionId)
    {
        return await _context.QuizAnswerOptions
            .FirstOrDefaultAsync(a => a.Id == answerOptionId);
    }

    public async Task<IEnumerable<QuizAnswerOption>>
        GetQuizAnswerOptionsByQuestionId(Guid quizQuestionId)
    {
        return await _context.QuizAnswerOptions
            .Where(a => a.QuizQuestionId == quizQuestionId)
            .ToListAsync();
    }

    public async Task<int> GetAnswerOptionCountByQuestionId(
        Guid quizQuestionId)
    {
        return await _context.QuizAnswerOptions
            .CountAsync(a => a.QuizQuestionId == quizQuestionId);
    }

    public async Task<bool> HasCorrectAnswer(Guid quizQuestionId)
    {
        return await _context.QuizAnswerOptions
            .AnyAsync(a =>
                a.QuizQuestionId == quizQuestionId &&
                a.IsCorrect);
    }

    public void DeleteQuizAnswerOption(
        QuizAnswerOption answerOption)
    {
        _context.QuizAnswerOptions.Remove(answerOption);
    }
}