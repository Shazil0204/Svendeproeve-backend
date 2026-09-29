using LMSBackend.Application.DTOs.Dashboard;
using LMSBackend.Application.DTOs.Subjects;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Enums.Tasks;
using LMSBackend.Domain.Enums.Users;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public sealed partial class DashboardReadRepository
{
    public async Task<TeacherDashboardDto?> GetTeacherAsync(Guid teacherId, TeacherDashboardQuery query,
        CancellationToken cancellationToken)
    {
        var teacher = await context.Users.AsNoTracking()
            .Where(t => t.Id == teacherId && t.Role == UserRole.Teacher && t.IsActive && !t.IsSoftDeleted)
            .Select(t => new { t.Id, t.Name, t.Email }).SingleOrDefaultAsync(cancellationToken);
        if (teacher == null) return null;

        var subjectsQuery = context.Subjects.AsNoTracking()
            .Where(s => !s.IsSoftDeleted && (!query.SubjectId.HasValue || s.Id == query.SubjectId.Value) &&
                (s.CreatedBy == teacherId || context.TeacherSubjects.Any(ts =>
                    ts.TeacherId == teacherId && ts.SubjectId == s.Id)));
        int subjectCount = await subjectsQuery.CountAsync(cancellationToken);
        var subjects = await subjectsQuery.OrderBy(s => s.Name).ThenBy(s => s.Id)
            .Skip((query.SubjectPage - 1) * query.PageSize).Take(query.PageSize)
            .Select(s => new SubjectResponseDTO(s.Id, s.Name, s.CreatedAt, s.CreatedBy))
            .ToListAsync(cancellationToken);

        // Assignment rows do not record who assigned them; ownership follows the task creator.
        var tasksQuery = context.Tasks.AsNoTracking()
            .Where(t => t.CreatedByUserId == teacherId && !t.IsSoftDeleted && !t.Subject.IsSoftDeleted &&
                (!query.SubjectId.HasValue || t.SubjectId == query.SubjectId.Value));
        int taskCount = await tasksQuery.CountAsync(cancellationToken);
        var tasks = await tasksQuery.OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Id)
            .Skip((query.TaskPage - 1) * query.PageSize).Take(query.PageSize)
            .Select(t => new TaskDto(t.Id, t.SubjectId, t.Title, t.Description, t.Deadline,
                t.CreatedAt, t.CreatedByUserId)).ToListAsync(cancellationToken);

        var taskIds = tasksQuery.Select(t => t.Id);
        var individualAssignments = context.TaskStudents.AsNoTracking()
            .Where(a => taskIds.Contains(a.TaskId) && a.Student.IsActive && !a.Student.IsSoftDeleted &&
                a.Student.Role == UserRole.Student)
            .Select(a => new TeacherAssignmentRow
            {
                AssignmentId = a.Id, TaskId = a.TaskId, Title = a.Task.Title, SubjectId = a.Task.SubjectId,
                RecipientType = "student", RecipientId = a.StudentId, RecipientName = a.Student.Name,
                Status = a.Status, AssignedAt = a.AssignedAt, Deadline = a.Task.Deadline
            });
        // Keep each group assignment as one row, regardless of how many students belong to the group.
        var groupAssignments = context.TaskGroups.AsNoTracking()
            .Where(a => taskIds.Contains(a.TaskId) && !a.Group.IsSoftDeleted)
            .Select(a => new TeacherAssignmentRow
            {
                AssignmentId = a.Id, TaskId = a.TaskId, Title = a.Task.Title, SubjectId = a.Task.SubjectId,
                RecipientType = "group", RecipientId = a.GroupId, RecipientName = a.Group.Name,
                Status = a.Status, AssignedAt = a.AssignedAt, Deadline = a.Task.Deadline
            });
        var assignmentsQuery = individualAssignments.Concat(groupAssignments);
        int assignmentCount = await assignmentsQuery.CountAsync(cancellationToken);
        var assignmentRows = await assignmentsQuery.OrderByDescending(a => a.AssignedAt)
            .ThenBy(a => a.AssignmentId)
            .Skip((query.AssignmentPage - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync(cancellationToken);
        var assignments = assignmentRows.Select(a => new TeacherDashboardAssignmentDto(a.AssignmentId,
            a.TaskId, a.Title, a.SubjectId, a.RecipientType, a.RecipientId, a.RecipientName,
            a.Status.ToString(), a.AssignedAt, a.Deadline)).ToList();

        return new(teacher.Id, teacher.Name, teacher.Email.Value,
            new(subjects, subjectCount, query.SubjectPage, query.PageSize),
            new(tasks, taskCount, query.TaskPage, query.PageSize),
            new(assignments, assignmentCount, query.AssignmentPage, query.PageSize));
    }

    private sealed class TeacherAssignmentRow
    {
        public Guid AssignmentId { get; init; }
        public Guid TaskId { get; init; }
        public string Title { get; init; } = string.Empty;
        public Guid SubjectId { get; init; }
        public string RecipientType { get; init; } = string.Empty;
        public Guid RecipientId { get; init; }
        public string RecipientName { get; init; } = string.Empty;
        public StudentTaskStatus Status { get; init; }
        public DateTimeOffset AssignedAt { get; init; }
        public DateTimeOffset? Deadline { get; init; }
    }
}
