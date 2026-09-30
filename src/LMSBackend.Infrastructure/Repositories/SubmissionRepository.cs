using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Domain.Entities.Submissions;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _context;
    public SubmissionRepository(AppDbContext context) => _context = context;

    public Task<TaskStudent?> GetStudentAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken) =>
        _context.TaskStudents.Include(a => a.Task)
            .SingleOrDefaultAsync(a => a.Id == assignmentId && !a.Task.IsSoftDeleted, cancellationToken);

    public Task<TaskGroup?> GetGroupAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken) =>
        _context.TaskGroups.Include(a => a.Task)
            .SingleOrDefaultAsync(a => a.Id == assignmentId && !a.Task.IsSoftDeleted, cancellationToken);

    public Task<Submission?> GetSubmissionAsync(Guid assignmentId, CancellationToken cancellationToken) =>
        _context.Submissions.SingleOrDefaultAsync(s => s.TaskStudentId == assignmentId || s.TaskGroupId == assignmentId, cancellationToken);

    public Task<SubmissionFile?> GetFileAsync(Guid submissionId, CancellationToken cancellationToken) =>
        _context.SubmissionFiles.AsNoTracking().SingleOrDefaultAsync(f => f.SubmissionId == submissionId, cancellationToken);

    public void Add(Submission submission, SubmissionFile? file)
    {
        _context.Submissions.Add(submission);
        if (file != null) _context.SubmissionFiles.Add(file);
    }

    public void Remove(TaskStudent assignment) => _context.TaskStudents.Remove(assignment);
    public void Remove(TaskGroup assignment) => _context.TaskGroups.Remove(assignment);
    public void AddFeedback(Feedback feedback) => _context.Feedback.Add(feedback);

    public async Task<IReadOnlyList<FeedbackDto>> GetFeedbackAsync(Guid studentId, Guid taskId, Guid[] assignmentIds, CancellationToken cancellationToken) =>
        await _context.Feedback.AsNoTracking().Where(f =>
            (f.Submission.TaskStudentId.HasValue && assignmentIds.Contains(f.Submission.TaskStudentId.Value)) ||
            (f.Submission.TaskGroupId.HasValue && assignmentIds.Contains(f.Submission.TaskGroupId.Value)))
            .OrderBy(f => f.CreatedAt).ThenBy(f => f.Id)
            .Select(f => new FeedbackDto(f.Id, studentId, taskId, f.Content, f.CreatedAt, f.TeacherId, f.Teacher.Name,
                f.SubmissionId, f.Submission.TaskStudentId ?? f.Submission.TaskGroupId!.Value))
            .ToListAsync(cancellationToken);
}
