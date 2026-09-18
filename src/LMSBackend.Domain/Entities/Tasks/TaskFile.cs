using LMSBackend.Domain.ValueObjects.Files;

namespace LMSBackend.Domain.Entities.Tasks;

public class TaskFile
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public FileName FileName { get; private set; } = null!;
    public FilePath FilePath { get; private set; } = null!;
    public DateTime UploadedAt { get; private set; }
    public Task Task { get; private set; } = null!;

    private TaskFile() { }

    public TaskFile(
        Guid taskId,
        FileName fileName,
        FilePath filePath)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        FileName = fileName;
        FilePath = filePath;
        UploadedAt = DateTime.UtcNow;
    }
}