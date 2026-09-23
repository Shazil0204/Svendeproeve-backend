using LMSBackend.Domain.Exceptions;

namespace LMSBackend.Domain.ValueObjects.Tasks;

public record class AssignmentTarget
{
    public Guid? TaskStudentId { get; }
    public Guid? TaskGroupId { get; }

    public AssignmentTarget(Guid? taskStudentId, Guid? taskGroupId)
    {
        if (taskStudentId.HasValue == taskGroupId.HasValue || taskStudentId == Guid.Empty || taskGroupId == Guid.Empty)
            throw new DomainValidationException("An assignment target must reference either a student or group assignment.");

        TaskStudentId = taskStudentId;
        TaskGroupId = taskGroupId;
    }

    public bool IsStudentAssignment => TaskStudentId.HasValue;

    public bool IsGroupAssignment => TaskGroupId.HasValue;
}
