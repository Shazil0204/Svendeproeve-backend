namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record PageDto<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
