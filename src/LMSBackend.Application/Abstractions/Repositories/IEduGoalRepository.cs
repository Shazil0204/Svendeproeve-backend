using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface IEduGoalRepository
{
    Task AddEduGoal(EducationalGoal eduGoal);
    Task<EducationalGoal?> GetEduGoalById(Guid eduGoalId);
    Task<List<EducationalGoal>> GetEduGoalsBySubjectId(Guid subjectId);
    Task<IEnumerable<EducationalGoal>> GetAllEduGoals();
    Task DeleteEduGoal(EducationalGoal eduGoal);
    Task<IReadOnlyList<EducationalGoal>> GetEduGoalBySubjectId(Guid subjectId);
}
