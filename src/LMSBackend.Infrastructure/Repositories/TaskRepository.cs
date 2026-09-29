using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskEntity = LMSBackend.Domain.Entities.Tasks.Task;
using TaskEducationalGoal = LMSBackend.Domain.Entities.Tasks.TaskEducationalGoal;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Tasks.AsNoTracking().Where(t => !t.IsSoftDeleted)
            .OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Id).ToListAsync(cancellationToken);

    public Task<TaskEntity?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken) =>
        _context.Tasks.SingleOrDefaultAsync(t => t.Id == taskId && !t.IsSoftDeleted, cancellationToken);

    public Task<bool> SubjectExistsAsync(Guid subjectId, CancellationToken cancellationToken) =>
        _context.Subjects.AnyAsync(s => s.Id == subjectId && !s.IsSoftDeleted, cancellationToken);

    public void Add(TaskEntity task) => _context.Tasks.Add(task);

    public Task<bool> HasSubmissionsAsync(Guid taskId, CancellationToken cancellationToken) =>
        _context.Submissions.AnyAsync(s =>
            (s.TaskStudent != null && s.TaskStudent.TaskId == taskId) ||
            (s.TaskGroup != null && s.TaskGroup.TaskId == taskId), cancellationToken);

    public Task<int> CountActiveStudentsAsync(Guid[] studentIds, CancellationToken cancellationToken) =>
        _context.Users.CountAsync(u => studentIds.Contains(u.Id) && !u.IsSoftDeleted &&
            u.IsActive && u.Role == UserRole.Student, cancellationToken);

    public Task<int> CountActiveGroupsAsync(Guid[] groupIds, CancellationToken cancellationToken) =>
        _context.StudentGroups.CountAsync(g => groupIds.Contains(g.Id) && !g.IsSoftDeleted, cancellationToken);

    public async Task<bool> AssignmentsExistAsync(Guid taskId, Guid[] studentIds, Guid[] groupIds,
        CancellationToken cancellationToken) =>
        await _context.TaskStudents.AnyAsync(a => a.TaskId == taskId && studentIds.Contains(a.StudentId), cancellationToken) ||
        await _context.TaskGroups.AnyAsync(a => a.TaskId == taskId && groupIds.Contains(a.GroupId), cancellationToken);

    public void AddAssignments(IEnumerable<TaskStudent> students, IEnumerable<TaskGroup> groups)
    {
        _context.TaskStudents.AddRange(students);
        _context.TaskGroups.AddRange(groups);
    }

    public async Task<IReadOnlyList<TaskAssignmentDto>> GetAssignmentsAsync(
        Guid? taskId, Guid? studentId, Guid? assignmentId, CancellationToken cancellationToken)
    {
        IQueryable<TaskStudent> students = _context.TaskStudents.AsNoTracking()
            .Where(a => !a.Task.IsSoftDeleted &&
                (!taskId.HasValue || a.TaskId == taskId.Value) &&
                (!assignmentId.HasValue || a.Id == assignmentId.Value) &&
                (!studentId.HasValue || (a.StudentId == studentId.Value &&
                    !a.Student.IsSoftDeleted && a.Student.IsActive)));
        IQueryable<TaskGroup> groups = _context.TaskGroups.AsNoTracking()
            .Where(a => !a.Task.IsSoftDeleted &&
                (!taskId.HasValue || a.TaskId == taskId.Value) &&
                (!assignmentId.HasValue || a.Id == assignmentId.Value) &&
                (!studentId.HasValue || (!a.Group.IsSoftDeleted &&
                    _context.GroupMemberships.Any(m => m.GroupId == a.GroupId &&
                        m.StudentId == studentId.Value && !m.Student.IsSoftDeleted && m.Student.IsActive))));

        List<TaskAssignmentDto> individualAssignments = await students.Select(a => new TaskAssignmentDto(
            a.Id, new TaskDto(a.Task.Id, a.Task.SubjectId, a.Task.Title, a.Task.Description,
                a.Task.Deadline, a.Task.CreatedAt, a.Task.CreatedByUserId),
            "student", a.StudentId, a.Student.Name, a.Task.Deadline, a.Status, a.AssignedAt))
            .ToListAsync(cancellationToken);
        List<TaskAssignmentDto> groupAssignments = await groups.Select(a => new TaskAssignmentDto(
            a.Id, new TaskDto(a.Task.Id, a.Task.SubjectId, a.Task.Title, a.Task.Description,
                a.Task.Deadline, a.Task.CreatedAt, a.Task.CreatedByUserId),
            "group", a.GroupId, a.Group.Name, a.Task.Deadline, a.Status, a.AssignedAt))
            .ToListAsync(cancellationToken);

        return individualAssignments.Concat(groupAssignments)
            .OrderByDescending(a => a.AssignedAt).ThenBy(a => a.AssignmentId).ToList();
    }

    public async Task<IReadOnlyList<EducationalGoal>> GetObjectivesAsync(Guid[] objectiveIds, CancellationToken cancellationToken) =>
        await _context.EducationalGoals.AsNoTracking()
            .Where(g => objectiveIds.Contains(g.Id) && !g.Subject.IsSoftDeleted).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TaskEducationalGoal>> GetObjectiveLinksAsync(Guid taskId, CancellationToken cancellationToken) =>
        await _context.TaskEducationalGoals.Include(g => g.EducationalGoal)
            .Where(g => g.TaskId == taskId).OrderBy(g => g.EducationalGoalId).ToListAsync(cancellationToken);

    public void ReplaceObjectives(Guid taskId, IReadOnlyList<TaskEducationalGoal> existing, Guid[] objectiveIds)
    {
        HashSet<Guid> requested = objectiveIds.ToHashSet();
        HashSet<Guid> previous = existing.Select(g => g.EducationalGoalId).ToHashSet();
        _context.TaskEducationalGoals.RemoveRange(existing.Where(g => !requested.Contains(g.EducationalGoalId)));
        _context.TaskEducationalGoals.AddRange(objectiveIds.Where(id => !previous.Contains(id))
            .Select(id => new TaskEducationalGoal(taskId, id)));
    }

    public async Task<IReadOnlyList<TaskEntity>> GetTasksBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        return await _context.Tasks.AsNoTracking()
            .Where(task => task.SubjectId == subjectId && !task.IsSoftDeleted)
            .OrderByDescending(task => task.CreatedAt).ThenBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }
}

