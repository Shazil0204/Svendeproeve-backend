namespace LMSBackend.Application.DTOs.Tasks;

public sealed record TaskDto(Guid Id, Guid SubjectId, string Title, string Description,
    DateTimeOffset? Deadline, DateTimeOffset CreatedAt, Guid CreatedByUserId);

