using System.IO.Compression;
using LMSBackend.Application.Abstractions.Submissions;
using LMSBackend.Application.Exceptions;

namespace LMSBackend.Infrastructure.Storage;

public sealed class LocalSubmissionFileStore : ISubmissionFileStore
{
    private readonly string _root;
    public LocalSubmissionFileStore(string root) => _root = Path.GetFullPath(root);

    public async Task<string> SaveZipAsync(Stream content, string fileName, CancellationToken cancellationToken)
    {
        if (!string.Equals(Path.GetExtension(fileName), ".zip", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Only ZIP files are accepted.");

        using var buffer = new MemoryStream();
        byte[] chunk = new byte[81920];
        int read;
        while ((read = await content.ReadAsync(chunk, cancellationToken)) != 0)
        {
            if (buffer.Length + read > ISubmissionFileStore.MaxFileBytes)
                throw new ValidationException("The ZIP file cannot exceed 20 MiB.");
            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }
        buffer.Position = 0;
        try
        {
            using var archive = new ZipArchive(buffer, ZipArchiveMode.Read, leaveOpen: true);
            _ = archive.Entries.Count; // Validate the archive directory; never extract uploaded files.
        }
        catch (InvalidDataException)
        {
            throw new ValidationException("The attachment must be a valid ZIP archive.");
        }

        string key = Guid.NewGuid().ToString("N") + ".zip";
        string path = Resolve(key);
        Directory.CreateDirectory(_root);
        try
        {
            await using var destination = new FileStream(path, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 81920, FileOptions.Asynchronous);
            buffer.Position = 0;
            await buffer.CopyToAsync(destination, cancellationToken);
        }
        catch
        {
            File.Delete(path);
            throw;
        }
        return key;
    }

    public Task<Stream> OpenReadAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            Stream stream = new FileStream(Resolve(key), FileMode.Open, FileAccess.Read,
                FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
            return Task.FromResult(stream);
        }
        catch (FileNotFoundException) { throw new NotFoundException("Submission file not found."); }
        catch (DirectoryNotFoundException) { throw new NotFoundException("Submission file not found."); }
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        File.Delete(Resolve(key));
        return Task.CompletedTask;
    }

    private string Resolve(string key)
    {
        if (key.Length != 36 || !key.EndsWith(".zip", StringComparison.Ordinal) ||
            !Guid.TryParseExact(key[..32], "N", out _))
            throw new NotFoundException("Submission file not found.");
        return Path.Combine(_root, key);
    }
}

