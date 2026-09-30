using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Tasks;

public sealed record SaveTaskRequest
{
    [Required]
    public Guid SubjectId { get; init; }

    [Required, StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(5000)]
    public string? Description { get; init; }

    public DateTimeOffset? Deadline { get; init; }
}

