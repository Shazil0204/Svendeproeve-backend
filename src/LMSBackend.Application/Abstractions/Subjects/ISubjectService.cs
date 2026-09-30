using LMSBackend.Application.DTOs.Subjects;
using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Abstractions.Subjects;

public interface ISubjectService
{
    Task AddSubject(CreateSubject createSubject);
    Task<SubjectResponseDTO?> GetSubjectById(Guid subjectId);
    Task<IEnumerable<SubjectResponseDTO>> GetAllSubjects();
    Task UpdateSubject(UpdateSubject updateSubject, Guid subjectId);
    Task SoftDeleteSubject(Guid subjectId);
    Task<IEnumerable<SubjectResponseDTO>> GetAllSubjectsByUserId();
}