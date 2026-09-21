using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Groups;

public sealed class GroupNameRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;
}

