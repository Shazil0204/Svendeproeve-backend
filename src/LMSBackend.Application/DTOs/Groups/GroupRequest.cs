using System.ComponentModel.DataAnnotations;

namespace LMSBackend.Application.DTOs.Groups;

public sealed record GroupRequest
{
    public string Name { get; init; } = string.Empty;
}

