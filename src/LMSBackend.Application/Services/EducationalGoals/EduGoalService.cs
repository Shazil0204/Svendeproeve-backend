using LMSBackend.Application.Abstractions.EducationalGoals;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.EducationalGoals;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Services.EducationalGoals;

public class EduGoalService : IEduGoalService
{
    private readonly IEduGoalRepository _eduGoalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EduGoalService(IEduGoalRepository eduGoalRepository, IUnitOfWork unitOfWork)
    {
        _eduGoalRepository = eduGoalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddEduGoal(CreateEduGoalRequest eduGoal)
    {
        EducationalGoal newEduGoal = new EducationalGoal(eduGoal.SubjectId, eduGoal.Content);
        await _eduGoalRepository.AddEduGoal(newEduGoal);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<EduGoalResponse?> GetEduGoalById(Guid eduGoalId)
    {
        EducationalGoal? eduGoal = await _eduGoalRepository.GetEduGoalById(eduGoalId) ?? throw new NotFoundException("Educational goal not found");
        return new EduGoalResponse(eduGoal.Id, eduGoal.SubjectId, eduGoal.Content);
    }

    public async Task<IEnumerable<EduGoalResponse>> GetAllEduGoals()
    {
        IEnumerable<EducationalGoal> eduGoals = await _eduGoalRepository.GetAllEduGoals();
        return eduGoals.Select(eduGoal => new EduGoalResponse(eduGoal.Id, eduGoal.SubjectId, eduGoal.Content));
    }

    public async Task UpdateEduGoalUpdateContent(Guid eduGoalId, UpdateEduGoalRequest upd)
    {
        EducationalGoal eduGoal = await _eduGoalRepository.GetEduGoalById(eduGoalId) ?? throw new Exception("Educational goal not found");
        eduGoal?.UpdateContent(upd.Content);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEduGoal(Guid eduGoalId)
    {
        EducationalGoal eduGoal = await _eduGoalRepository.GetEduGoalById(eduGoalId) ?? throw new Exception("Educational goal not found");
        await _eduGoalRepository.DeleteEduGoal(eduGoal);
        await _unitOfWork.SaveChangesAsync();
    }
}
