namespace LMSBackend.Application.DTOs.EducationalGoals;

public sealed record CreateEduGoalRequest
(
    Guid SubjectId,
    string Content
);