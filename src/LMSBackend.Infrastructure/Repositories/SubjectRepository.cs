using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddSubject(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
    }

    public async Task<Subject?> GetSubjectById(Guid subjectId)
    {
        return await _context.Subjects.FirstOrDefaultAsync(subject =>
            subject.Id == subjectId && !subject.IsSoftDeleted);
    }

    public async Task<IEnumerable<Subject>> GetAllSubjectsByUserId(Guid userId)
    {
        return await _context.Subjects
        .Where(subject => subject.CreatedBy == userId && !subject.IsSoftDeleted)
        .ToListAsync();
    }

    public async Task<IEnumerable<Subject>> GetAllSubjects()
    {
        return await _context.Subjects
        .Where(subject => !subject.IsSoftDeleted)
        .ToListAsync();
    }
}
