namespace LMSBackend.Application.Services.EducationalGoals;

public sealed record EduGoalResponse
(
    Guid Id,
    Guid SubjectId,
    string Content
);
