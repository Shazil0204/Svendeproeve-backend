namespace LMSBackend.Application.DTOs.Groups;

public record GroupDetailsDto(Guid Id, string Name, DateTimeOffset CreatedAt, IReadOnlyList<GroupStudentDto> Students);
