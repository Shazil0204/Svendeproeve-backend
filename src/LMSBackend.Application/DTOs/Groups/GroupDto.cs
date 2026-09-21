namespace LMSBackend.Application.DTOs.Groups;

public record GroupDto(Guid Id, string Name, DateTimeOffset CreatedAt);
