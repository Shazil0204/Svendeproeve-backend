using LMSBackend.Domain.ValueObjects.Files;

namespace LMSBackend.Domain.Entities.Submissions;

public class SubmissionFile
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public FileName FileName { get; private set; } = null!;
    public FilePath FilePath { get; private set; } = null!;
    public DateTimeOffset UploadedAt { get; private set; }
    public Submission Submission { get; private set; } = null!;

    private SubmissionFile() { }

    public SubmissionFile(
        Guid submissionId,
        FileName fileName,
        FilePath filePath)
    {
        Id = Guid.NewGuid();
        SubmissionId = submissionId;
        FileName = fileName;
        FilePath = filePath;
        UploadedAt = DateTimeOffset.UtcNow;
    }
}