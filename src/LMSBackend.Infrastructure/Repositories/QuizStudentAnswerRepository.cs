using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class QuizStudentAnswerRepository : IQuizStudentAnswerRepository
{
    private readonly AppDbContext _context;

    public QuizStudentAnswerRepository(AppDbContext context)
    {
        _context = context;
    }

    // Quiz Student

    public async Task AddQuizStudent(QuizStudent quizStudent)
    {
        await _context.QuizStudents.AddAsync(quizStudent);
    }

    public async Task<QuizStudent?> GetQuizStudent(
        Guid quizId,
        Guid studentId)
    {
        return await _context.QuizStudents
            .FirstOrDefaultAsync(qs =>
                qs.QuizId == quizId &&
                qs.StudentId == studentId);
    }

    public async Task<IEnumerable<QuizStudent>>
        GetQuizStudentsByQuizId(Guid quizId)
    {
        return await _context.QuizStudents
            .Where(qs => qs.QuizId == quizId)
            .ToListAsync();
    }

    public void DeleteQuizStudent(QuizStudent quizStudent)
    {
        _context.QuizStudents.Remove(quizStudent);
    }

    // Student Answers

    public async Task AddStudentQuizAnswer(QuizAnswer quizAnswer)
    {
        await _context.QuizAnswers.AddAsync(quizAnswer);
    }

    public async Task<bool> HasAnsweredQuestion(
        Guid quizStudentId,
        Guid quizQuestionId)
    {
        return await _context.QuizAnswers
            .AnyAsync(a =>
                a.QuizStudentId == quizStudentId &&
                a.QuizQuestionId == quizQuestionId);
    }

    public async Task<IEnumerable<QuizAnswer>>
        GetStudentAnswersByQuizStudentId(Guid quizStudentId)
    {
        return await _context.QuizAnswers
            .Where(a => a.QuizStudentId == quizStudentId)
            .ToListAsync();
    }

    public async Task<QuizAnswer?> GetStudentAnswerByQuestionId(
        Guid quizStudentId,
        Guid quizQuestionId)
    {
        return await _context.QuizAnswers
            .FirstOrDefaultAsync(a =>
                a.QuizStudentId == quizStudentId &&
                a.QuizQuestionId == quizQuestionId);
    }
}