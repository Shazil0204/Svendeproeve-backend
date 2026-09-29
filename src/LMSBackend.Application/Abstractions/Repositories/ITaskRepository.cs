using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Entities.Subjects;
using TaskEntity = LMSBackend.Domain.Entities.Tasks.Task;
using TaskEducationalGoal = LMSBackend.Domain.Entities.Tasks.TaskEducationalGoal;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskEntity>> GetTasksBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken);
    Task<TaskEntity?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken);
    Task<bool> SubjectExistsAsync(Guid subjectId, CancellationToken cancellationToken);
    void Add(TaskEntity task);
    Task<bool> HasSubmissionsAsync(Guid taskId, CancellationToken cancellationToken);
    Task<int> CountActiveStudentsAsync(Guid[] studentIds, CancellationToken cancellationToken);
    Task<int> CountActiveGroupsAsync(Guid[] groupIds, CancellationToken cancellationToken);
    Task<bool> AssignmentsExistAsync(Guid taskId, Guid[] studentIds, Guid[] groupIds, CancellationToken cancellationToken);
    void AddAssignments(IEnumerable<TaskStudent> students, IEnumerable<TaskGroup> groups);
    Task<IReadOnlyList<TaskAssignmentDto>> GetAssignmentsAsync(Guid? taskId, Guid? studentId, Guid? assignmentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EducationalGoal>> GetObjectivesAsync(Guid[] objectiveIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskEducationalGoal>> GetObjectiveLinksAsync(Guid taskId, CancellationToken cancellationToken);
    void ReplaceObjectives(Guid taskId, IReadOnlyList<TaskEducationalGoal> existing, Guid[] objectiveIds);
}

