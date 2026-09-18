using LMSBackend.Domain.Entities.Subjects;

namespace LMSBackend.Domain.Entities.Tasks;

public class TaskEducationalGoal
{
    public Guid TaskId { get; private set; }
    public Guid EducationalGoalId { get; private set; }
    public Task Task { get; private set; } = null!;
    public EducationalGoal EducationalGoal { get; private set; } = null!;
    private TaskEducationalGoal() { }

    public TaskEducationalGoal(Guid taskId, Guid educationalGoalId)
    {
        TaskId = taskId;
        EducationalGoalId = educationalGoalId;
    }
}