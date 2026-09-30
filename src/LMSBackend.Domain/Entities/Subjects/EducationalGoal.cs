namespace LMSBackend.Domain.Entities.Subjects;

public class EducationalGoal
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Subject Subject { get; private set; } = null!;

    private EducationalGoal() { }

    public EducationalGoal(Guid subjectId, string content)
    {
        Id = Guid.NewGuid();
        SubjectId = subjectId;
        Content = content;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
    }
}
