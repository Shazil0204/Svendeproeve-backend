using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Domain.Entities.Groups;

public class GroupMembership
{
    public Guid GroupId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }
    public StudentGroup Group { get; private set; } = null!;
    public User Student { get; private set; } = null!;

    private GroupMembership() { }

    public GroupMembership(Guid groupId, Guid studentId)
    {
        GroupId = groupId;
        StudentId = studentId;
        AddedAt = DateTimeOffset.UtcNow;
    }
}