using LMSBackend.Application.DTOs.EducationalGoals;
using LMSBackend.Application.Services.EducationalGoals;
using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Abstractions.EducationalGoals;

public interface IEduGoalService
{
    Task AddEduGoal(CreateEduGoalRequest eduGoal);
    Task<EduGoalResponse?> GetEduGoalById(Guid eduGoalId);
    Task<List<EduGoalResponse>> GetEduGoalsBySubjectId(Guid subjectId);
    Task<IEnumerable<EduGoalResponse>> GetAllEduGoals();
    Task UpdateEduGoalUpdateContent(Guid eduGoalId, UpdateEduGoalRequest upd);
    Task DeleteEduGoal(Guid eduGoalId);
}
