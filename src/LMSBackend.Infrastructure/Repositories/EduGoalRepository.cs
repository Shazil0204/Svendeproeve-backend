using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public class EduGoalRepository : IEduGoalRepository
{
    private readonly AppDbContext _context;

    public EduGoalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddEduGoal(EducationalGoal eduGoal)
    {
        await _context.EducationalGoals.AddAsync(eduGoal);
    }

    public async Task<EducationalGoal?> GetEduGoalById(Guid eduGoalId)
    {
        return await _context.EducationalGoals.FindAsync(eduGoalId);
    }

    public async Task<IEnumerable<EducationalGoal>> GetAllEduGoals()
    {
        return await _context.EducationalGoals.ToListAsync();
    }

    public async Task DeleteEduGoal(EducationalGoal eduGoal)
    {
        // Stage both removals so the unit of work saves them atomically.
        var links = await _context.TaskEducationalGoals
            .Where(link => link.EducationalGoalId == eduGoal.Id).ToListAsync();
        _context.TaskEducationalGoals.RemoveRange(links);
        _context.EducationalGoals.Remove(eduGoal);
    }
}
