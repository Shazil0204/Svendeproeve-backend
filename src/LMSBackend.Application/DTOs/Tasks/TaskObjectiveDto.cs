namespace LMSBackend.Application.DTOs.Tasks;

public sealed record TaskObjectiveDto(Guid ObjectiveId, Guid SubjectId, string Content);

