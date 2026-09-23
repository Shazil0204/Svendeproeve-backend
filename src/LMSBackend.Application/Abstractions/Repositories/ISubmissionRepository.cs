using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Domain.Entities.Submissions;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Application.Abstractions.Repositories;

public interface ISubmissionRepository
{
    Task<TaskStudent?> GetStudentAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken);
    Task<TaskGroup?> GetGroupAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken);
    Task<Submission?> GetSubmissionAsync(Guid assignmentId, CancellationToken cancellationToken);
    Task<SubmissionFile?> GetFileAsync(Guid submissionId, CancellationToken cancellationToken);
    void Add(Submission submission, SubmissionFile? file);
    void Remove(TaskStudent assignment);
    void Remove(TaskGroup assignment);
    void AddFeedback(Feedback feedback);
    Task<IReadOnlyList<FeedbackDto>> GetFeedbackAsync(Guid studentId, Guid taskId, Guid[] assignmentIds, CancellationToken cancellationToken);
}
