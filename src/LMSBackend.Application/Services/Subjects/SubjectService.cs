using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Subjects;
using LMSBackend.Application.DTOs.Subjects;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Services.Subjects;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public SubjectService(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task AddSubject(CreateSubject createSubject)
    {
        Guid createdBy = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated");
        Subject subject = new(createSubject.Name, createdBy);
        await _subjectRepository.AddSubject(subject);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SubjectResponseDTO?> GetSubjectById(Guid subjectId)
    {
        Subject? subject = await _subjectRepository.GetSubjectById(subjectId);
        if (subject == null)
        {
            return null;
        }
        return new SubjectResponseDTO(subject.Id, subject.Name, subject.CreatedAt, subject.CreatedBy);
    }

    public async Task<IEnumerable<SubjectResponseDTO>> GetAllSubjects()
    {
        IEnumerable<Subject> subjects = await _subjectRepository.GetAllSubjects();
        return subjects.Select(subject => new SubjectResponseDTO(subject.Id, subject.Name, subject.CreatedAt, subject.CreatedBy));
    }

    public async Task UpdateSubject(UpdateSubject updateSubject, Guid subjectId)
    {
        SubjectResponseDTO? subject = await GetSubjectById(subjectId) ?? throw new NotFoundException("Subject not found");
        Subject subjectToUpdate = await _subjectRepository.GetSubjectById(subjectId) ?? throw new NotFoundException("Subject not found");
        subjectToUpdate.UpdateName(updateSubject.Name);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SoftDeleteSubject(Guid subjectId)
    {
        SubjectResponseDTO? subject = await GetSubjectById(subjectId) ?? throw new NotFoundException("Subject not found");
        Subject subjectToDelete = await _subjectRepository.GetSubjectById(subjectId) ?? throw new NotFoundException("Subject not found");
        subjectToDelete.SoftDelete();
        await _unitOfWork.SaveChangesAsync();
    }
}
