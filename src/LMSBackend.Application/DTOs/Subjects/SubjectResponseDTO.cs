namespace LMSBackend.Application.DTOs.Subjects;

public sealed record SubjectResponseDTO
(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt,
    Guid CreatedBy
);
