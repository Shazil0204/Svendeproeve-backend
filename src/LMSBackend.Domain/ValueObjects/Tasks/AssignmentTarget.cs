namespace LMSBackend.Domain.ValueObjects.Tasks;

public record class AssignmentTarget
{
    public Guid? TaskStudentId { get; }
    public Guid? TaskGroupId { get; }

    public AssignmentTarget(Guid? taskStudentId, Guid? taskGroupId)
    {
        if (taskStudentId is null && taskGroupId is null)
            throw new ArgumentException(
                "An assignment target must reference either a student or group assignment.");

        TaskStudentId = taskStudentId;
        TaskGroupId = taskGroupId;
    }

    public bool IsStudentAssignment => TaskStudentId.HasValue;

    public bool IsGroupAssignment => TaskGroupId.HasValue;
}