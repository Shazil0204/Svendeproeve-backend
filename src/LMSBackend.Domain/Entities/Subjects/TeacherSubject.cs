using LMSBackend.Domain.Entities.Users;

namespace LMSBackend.Domain.Entities.Subjects;

public class TeacherSubject
{
    public Guid TeacherId { get; private set; }
    public Guid SubjectId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public User Teacher { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;

    private TeacherSubject() { }

    public TeacherSubject(Guid teacherId, Guid subjectId)
    {
        TeacherId = teacherId;
        SubjectId = subjectId;
        AssignedAt = DateTime.UtcNow;
    }
}