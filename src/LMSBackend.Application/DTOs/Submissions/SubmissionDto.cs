namespace LMSBackend.Application.DTOs.Submissions;

public sealed record SubmissionDto(Guid Id, Guid AssignmentId, string? Comment,
    DateTimeOffset SubmittedAt, Guid SubmittedByUserId, string Status, SubmissionFileDto? File);
public sealed record SubmissionFileDto(Guid Id, string FileName, DateTimeOffset UploadedAt);
public sealed record SubmissionDownload(Stream Content, string FileName);

