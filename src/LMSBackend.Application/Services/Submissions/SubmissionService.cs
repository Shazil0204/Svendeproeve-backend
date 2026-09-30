using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Submissions;
using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Submissions;
using LMSBackend.Domain.Enums.Tasks;
using LMSBackend.Domain.ValueObjects.Files;
using LMSBackend.Domain.ValueObjects.Tasks;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Application.Services.Submissions;

public sealed class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _repository;
    private readonly ITaskRepository _tasks;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ISubmissionFileStore _files;

    public SubmissionService(ISubmissionRepository repository, ITaskRepository tasks, IUnitOfWork unitOfWork,
        ICurrentUserService currentUser, ISubmissionFileStore files)
    {
        _repository = repository;
        _tasks = tasks;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _files = files;
    }

    public async Task<SubmissionDto> SubmitAsync(Guid assignmentId, string? comment, Stream? file,
        string? fileName, CancellationToken cancellationToken)
    {
        var assignment = await RequireAssignmentAsync(assignmentId, false, cancellationToken);
        if (await _repository.GetSubmissionAsync(assignmentId, cancellationToken) != null ||
            assignment.Status != StudentTaskStatus.NotSubmitted)
            throw new ConflictException("This assignment already has a submission.");
        if (comment is null || comment.Length > 5000)
            throw new ValidationException("Comment is required and cannot exceed 5000 characters.");

        string? safeName = null;
        if (file != null)
        {
            safeName = Path.GetFileName(fileName?.Replace('\\', '/') ?? string.Empty);
            if (string.IsNullOrWhiteSpace(safeName) || safeName.Length > 255)
                throw new ValidationException("A file name of at most 255 characters is required.");
        }

        var submission = new Submission(new AssignmentTarget(assignment.Student?.Id, assignment.Group?.Id), UserId, comment.Trim());
        SubmissionFile? attachment = null;
        string? storedKey = null;
        try
        {
            if (file != null)
            {
                storedKey = await _files.SaveZipAsync(file, safeName!, cancellationToken);
                attachment = new SubmissionFile(submission.Id, new FileName(safeName!), new FilePath(storedKey));
            }
            _repository.Add(submission, attachment);
            assignment.SetStatus(StudentTaskStatus.Submitted);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (storedKey != null)
                await _files.DeleteAsync(storedKey, CancellationToken.None);
            throw;
        }
        return ToDto(submission, assignment, attachment);
    }

    public async Task<SubmissionDto> GetAsync(Guid assignmentId, bool isTeacher, CancellationToken cancellationToken)
    {
        var assignment = await RequireAssignmentAsync(assignmentId, isTeacher, cancellationToken);
        var submission = await RequireSubmissionAsync(assignmentId, cancellationToken);
        return ToDto(submission, assignment, await _repository.GetFileAsync(submission.Id, cancellationToken));
    }

    public async Task<SubmissionDownload> DownloadAsync(Guid assignmentId, bool isTeacher, CancellationToken cancellationToken)
    {
        await RequireAssignmentAsync(assignmentId, isTeacher, cancellationToken);
        var submission = await RequireSubmissionAsync(assignmentId, cancellationToken);
        var file = await _repository.GetFileAsync(submission.Id, cancellationToken)
            ?? throw new NotFoundException("Submission file not found.");
        return new SubmissionDownload(await _files.OpenReadAsync(file.FilePath.Value, cancellationToken), file.FileName.Value);
    }

    public async Task<SubmissionDto> ReviewAsync(Guid assignmentId, ReviewSubmissionRequest request, CancellationToken cancellationToken)
    {
        StudentTaskStatus status = request.Status?.Trim() switch
        {
            "Godkendt" or "Approved" => StudentTaskStatus.Approved,
            "Ikke godkendt" or "Rejected" => StudentTaskStatus.Rejected,
            _ => throw new ValidationException("Status must be Godkendt or Ikke godkendt.")
        };
        var assignment = await RequireAssignmentAsync(assignmentId, true, cancellationToken);
        var submission = await RequireSubmissionAsync(assignmentId, cancellationToken);
        if (assignment.Status != StudentTaskStatus.Submitted)
            throw new ConflictException("Only a submitted assignment can be reviewed.");
        assignment.SetStatus(status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(submission, assignment, await _repository.GetFileAsync(submission.Id, cancellationToken));
    }

    public async Task RemoveAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken)
    {
        var assignment = await RequireAssignmentAsync(assignmentId, true, cancellationToken);
        if (await _repository.GetSubmissionAsync(assignmentId, cancellationToken) != null ||
            assignment.Status != StudentTaskStatus.NotSubmitted)
            throw new ConflictException("An assignment with a submission cannot be removed.");
        if (assignment.Student != null) _repository.Remove(assignment.Student);
        else _repository.Remove(assignment.Group!);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<FeedbackDto> AddFeedbackAsync(Guid studentId, Guid taskId, CreateFeedbackRequest request,
        CancellationToken cancellationToken)
    {
        Guid[] assignmentIds = await RequireStudentTaskAsync(studentId, taskId, true, cancellationToken);
        string? text = request.Text?.Trim();
        if (string.IsNullOrEmpty(text) || text.Length > 5000)
            throw new ValidationException("Feedback must contain 1 to 5000 characters.");
        if (request.AssignmentId.HasValue)
        {
            if (!assignmentIds.Contains(request.AssignmentId.Value))
                throw new NotFoundException("Student assignment not found.");
            assignmentIds = [request.AssignmentId.Value];
        }
        var submissions = new List<Submission>();
        foreach (Guid assignmentId in assignmentIds)
        {
            var submission = await _repository.GetSubmissionAsync(assignmentId, cancellationToken);
            if (submission != null) submissions.Add(submission);
        }
        if (submissions.Count == 0)
            throw new NotFoundException("The student has no submission for this task.");
        if (submissions.Count > 1)
            throw new ValidationException("More than one submission is accessible. Specify assignmentId for the feedback.");
        var feedback = new Feedback(submissions[0].Id, UserId, text);
        _repository.AddFeedback(feedback);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return (await _repository.GetFeedbackAsync(studentId, taskId, assignmentIds, cancellationToken)).Single(f => f.Id == feedback.Id);
    }

    public async Task<IReadOnlyList<FeedbackDto>> GetFeedbackAsync(Guid studentId, Guid taskId, bool isTeacher,
        CancellationToken cancellationToken)
    {
        Guid[] assignmentIds = await RequireStudentTaskAsync(studentId, taskId, isTeacher, cancellationToken);
        return await _repository.GetFeedbackAsync(studentId, taskId, assignmentIds, cancellationToken);
    }

    private async Task<Guid[]> RequireStudentTaskAsync(Guid studentId, Guid taskId, bool isTeacher, CancellationToken cancellationToken)
    {
        Guid userId = UserId;
        if (!isTeacher && userId != studentId)
            throw new NotFoundException("Student task not found.");
        var assignments = await _tasks.GetAssignmentsAsync(taskId, studentId, null, cancellationToken);
        if (assignments.Count == 0)
            throw new NotFoundException("Student task not found.");
        return assignments.Select(a => a.AssignmentId).ToArray();
    }

    private async Task<Assignment> RequireAssignmentAsync(Guid assignmentId, bool isTeacher, CancellationToken cancellationToken)
    {
        Guid userId = UserId;
        if (!isTeacher && (await _tasks.GetAssignmentsAsync(null, userId, assignmentId, cancellationToken)).Count == 0)
            throw new NotFoundException("Assignment not found.");
        var student = await _repository.GetStudentAssignmentAsync(assignmentId, cancellationToken);
        if (student != null) return new Assignment(student, null);
        var group = await _repository.GetGroupAssignmentAsync(assignmentId, cancellationToken);
        return group != null ? new Assignment(null, group) : throw new NotFoundException("Assignment not found.");
    }

    private async Task<Submission> RequireSubmissionAsync(Guid assignmentId, CancellationToken cancellationToken) =>
        await _repository.GetSubmissionAsync(assignmentId, cancellationToken) ?? throw new NotFoundException("Submission not found.");

    private Guid UserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");

    private static SubmissionDto ToDto(Submission submission, Assignment assignment, SubmissionFile? file) =>
        new(submission.Id, assignment.Student?.Id ?? assignment.Group!.Id, submission.Comment, submission.SubmittedAt,
            submission.SubmittedByUserId, assignment.Status switch
            {
                StudentTaskStatus.Submitted => "Afleveret",
                StudentTaskStatus.Approved => "Godkendt",
                StudentTaskStatus.Rejected => "Ikke godkendt",
                _ => "Ikke afleveret"
            }, file == null ? null : new SubmissionFileDto(file.Id, file.FileName.Value, file.UploadedAt));

    private sealed record Assignment(TaskStudent? Student, TaskGroup? Group)
    {
        public StudentTaskStatus Status => Student?.Status ?? Group!.Status;
        public void SetStatus(StudentTaskStatus status)
        {
            if (Student != null) Student.UpdateStatus(status);
            else Group!.UpdateStatus(status);
        }
    }
}
