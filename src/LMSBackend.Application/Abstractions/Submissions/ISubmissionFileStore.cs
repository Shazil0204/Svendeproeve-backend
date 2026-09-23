namespace LMSBackend.Application.Abstractions.Submissions;

public interface ISubmissionFileStore
{
    const long MaxFileBytes = 20 * 1024 * 1024;
    Task<string> SaveZipAsync(Stream content, string fileName, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(string key, CancellationToken cancellationToken);
    Task DeleteAsync(string key, CancellationToken cancellationToken);
}

