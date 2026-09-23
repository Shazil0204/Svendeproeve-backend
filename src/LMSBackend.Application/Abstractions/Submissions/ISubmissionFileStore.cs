namespace LMSBackend.Application.Abstractions.Submissions;

public interface ISubmissionFileStore
{
    long MaxFileBytes { get; }
    Task<string> SaveZipAsync(Stream content, string fileName, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(string key, CancellationToken cancellationToken);
    Task DeleteAsync(string key, CancellationToken cancellationToken);
}
