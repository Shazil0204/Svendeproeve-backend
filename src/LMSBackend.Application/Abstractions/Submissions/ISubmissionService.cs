using LMSBackend.Application.DTOs.Submissions;

namespace LMSBackend.Application.Abstractions.Submissions;

public interface ISubmissionService
{
    Task<SubmissionDto> SubmitAsync(Guid assignmentId, string? comment, Stream? file, string? fileName, CancellationToken cancellationToken);
    Task<SubmissionDto> GetAsync(Guid assignmentId, bool isTeacher, CancellationToken cancellationToken);
    Task<SubmissionDownload> DownloadAsync(Guid assignmentId, bool isTeacher, CancellationToken cancellationToken);
    Task<SubmissionDto> ReviewAsync(Guid assignmentId, ReviewSubmissionRequest request, CancellationToken cancellationToken);
    Task RemoveAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken);
    Task<FeedbackDto> AddFeedbackAsync(Guid studentId, Guid taskId, CreateFeedbackRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<FeedbackDto>> GetFeedbackAsync(Guid studentId, Guid taskId, bool isTeacher, CancellationToken cancellationToken);
}

