using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface ISubjectRepository
{
    Task AddSubject(Subject subject);
    Task<Subject?> GetSubjectById(Guid subjectId);
    Task<IEnumerable<Subject>> GetAllSubjects();
    Task<IEnumerable<Subject>> GetAllSubjectsByUserId(Guid userId);
}
