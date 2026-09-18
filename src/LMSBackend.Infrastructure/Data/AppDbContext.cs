using LMSBackend.Domain.Entities.Auditing;
using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Domain.Entities.Submissions;
using LMSBackend.Domain.Entities.Tasks;
using LMSBackend.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<GroupMembership> GroupMemberships => Set<GroupMembership>();
    public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();
    public DbSet<QuizAnswerOption> QuizAnswerOptions => Set<QuizAnswerOption>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizStudent> QuizStudents => Set<QuizStudent>();
    public DbSet<EducationalGoal> EducationalGoals => Set<EducationalGoal>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<Feedback> Feedback => Set<Feedback>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<SubmissionFile> SubmissionFiles => Set<SubmissionFile>();
    public DbSet<LMSBackend.Domain.Entities.Tasks.Task> Tasks => Set<LMSBackend.Domain.Entities.Tasks.Task>();
    public DbSet<TaskEducationalGoal> TaskEducationalGoals => Set<TaskEducationalGoal>();
    public DbSet<TaskFile> TaskFiles => Set<TaskFile>();
    public DbSet<TaskGroup> TaskGroups => Set<TaskGroup>();
    public DbSet<TaskStudent> TaskStudents => Set<TaskStudent>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserConsent> UserConsents => Set<UserConsent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
