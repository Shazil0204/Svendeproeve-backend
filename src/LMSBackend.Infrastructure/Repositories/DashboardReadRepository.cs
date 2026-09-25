using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Dashboard;
using LMSBackend.Application.DTOs.Submissions;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Quizzes;
using LMSBackend.Domain.Enums.Tasks;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Progression;
using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSBackend.Infrastructure.Repositories;

public sealed class DashboardReadRepository(AppDbContext context) : IDashboardReadRepository
{
    private IQueryable<User> Students => context.Users.AsNoTracking()
        .Where(s => s.Role == UserRole.Student && s.IsActive && !s.IsSoftDeleted);

    // One row per student/assignment. Group credit follows current membership.
    // Individual and group assignments of the same task remain separate obligations.
    private IQueryable<AssignmentRow> Assignments(Guid? subjectId)
    {
        var individual = context.TaskStudents.AsNoTracking()
            .Where(a => !a.Task.IsSoftDeleted && !a.Task.Subject.IsSoftDeleted &&
                a.Student.IsActive && !a.Student.IsSoftDeleted && a.Student.Role == UserRole.Student &&
                (!subjectId.HasValue || a.Task.SubjectId == subjectId.Value))
            .Select(a => new AssignmentRow
            {
                StudentId = a.StudentId, AssignmentId = a.Id, TaskId = a.TaskId,
                Title = a.Task.Title, SubjectId = a.Task.SubjectId, GroupId = null,
                Status = a.Status, AssignedAt = a.AssignedAt, Deadline = a.Task.Deadline
            });
        var groups = from a in context.TaskGroups.AsNoTracking()
                     join m in context.GroupMemberships on a.GroupId equals m.GroupId
                     where !a.Task.IsSoftDeleted && !a.Task.Subject.IsSoftDeleted && !a.Group.IsSoftDeleted &&
                         m.Student.IsActive && !m.Student.IsSoftDeleted && m.Student.Role == UserRole.Student &&
                         (!subjectId.HasValue || a.Task.SubjectId == subjectId.Value)
                     select new AssignmentRow
                     {
                         StudentId = m.StudentId, AssignmentId = a.Id, TaskId = a.TaskId,
                         Title = a.Task.Title, SubjectId = a.Task.SubjectId, GroupId = a.GroupId,
                         Status = a.Status, AssignedAt = a.AssignedAt, Deadline = a.Task.Deadline
                     };
        return individual.Concat(groups);
    }

    private IQueryable<QuizStudent> Quizzes(Guid? subjectId) => context.QuizStudents.AsNoTracking()
        .Where(a => !a.Quiz.IsSoftDeleted && !a.Quiz.Subject.IsSoftDeleted &&
            a.Student.IsActive && !a.Student.IsSoftDeleted && a.Student.Role == UserRole.Student &&
            (!subjectId.HasValue || a.Quiz.SubjectId == subjectId.Value));

    public async Task<PageDto<StudentProgressDto>> ListStudentsAsync(StudentDashboardQuery query,
        CancellationToken cancellationToken)
    {
        var students = Students.Where(s => !query.GroupId.HasValue || context.GroupMemberships.Any(m =>
            m.StudentId == s.Id && m.GroupId == query.GroupId.Value && !m.Group.IsSoftDeleted));
        int total = await students.CountAsync(cancellationToken);
        var page = await students.OrderBy(s => s.Name).ThenBy(s => s.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(s => new { s.Id, s.Name }).ToListAsync(cancellationToken);
        var summaries = await GetSummariesAsync(page.Select(s => s.Id).ToArray(), query.SubjectId, cancellationToken);
        return new(page.Select(s => new StudentProgressDto(s.Id, s.Name, summaries[s.Id])).ToList(),
            total, query.Page, query.PageSize);
    }

    public async Task<StudentDashboardDto?> GetStudentAsync(Guid studentId, DashboardQuery query,
        CancellationToken cancellationToken)
    {
        var student = await Students.Where(s => s.Id == studentId)
            .Select(s => new { s.Id, s.Name }).SingleOrDefaultAsync(cancellationToken);
        if (student == null) return null;

        var summaries = await GetSummariesAsync([studentId], query.SubjectId, cancellationToken);
        var summary = summaries[studentId];
        var assignments = Assignments(query.SubjectId).Where(a => a.StudentId == studentId);
        var tasks = await TaskPreviews(assignments.OrderByDescending(a => a.AssignedAt).ThenBy(a => a.AssignmentId)
            .Skip((query.TaskPage - 1) * query.PageSize).Take(query.PageSize)).ToListAsync(cancellationToken);

        // Materialize the converted Percentage value before reading .Value.
        var quizRows = await Quizzes(query.SubjectId).Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AssignedAt).ThenBy(a => a.Id)
            .Skip((query.QuizPage - 1) * query.PageSize).Take(query.PageSize)
            .Select(a => new { a.Id, a.QuizId, a.Quiz.Title, a.Quiz.SubjectId, a.Status,
                a.ScorePercentage, a.Passed, a.AssignedAt, a.CompletedAt }).ToListAsync(cancellationToken);
        var quizzes = quizRows.Select(a => new DashboardQuizDto(a.Id, a.QuizId, a.Title, a.SubjectId,
            a.Status.ToString(), a.ScorePercentage?.Value, a.Passed, a.AssignedAt, a.CompletedAt)).ToList();

        var feedbackQuery = FeedbackFor(assignments);
        int feedbackCount = await feedbackQuery.CountAsync(cancellationToken);
        var feedback = await feedbackQuery.OrderByDescending(f => f.CreatedAt).ThenBy(f => f.Id)
            .Skip((query.FeedbackPage - 1) * query.PageSize).Take(query.PageSize)
            .Select(f => new DashboardFeedbackDto(f.Id, f.TaskId, f.TaskTitle, f.AssignmentId,
                f.SubmissionId, f.Text, f.CreatedAt, f.TeacherId, f.TeacherName)).ToListAsync(cancellationToken);

        return new(student.Id, student.Name, summary,
            new(tasks, summary.Tasks.Assigned, query.TaskPage, query.PageSize),
            new(quizzes, summary.Quizzes.Assigned, query.QuizPage, query.PageSize),
            new(feedback, feedbackCount, query.FeedbackPage, query.PageSize));
    }

    private async Task<Dictionary<Guid, ProgressSummaryDto>> GetSummariesAsync(Guid[] studentIds,
        Guid? subjectId, CancellationToken cancellationToken)
    {
        if (studentIds.Length == 0) return [];
        var tasks = await Assignments(subjectId).Where(a => studentIds.Contains(a.StudentId))
            .GroupBy(a => new { a.StudentId, a.Status })
            .Select(g => new { g.Key.StudentId, g.Key.Status, Count = g.Count() }).ToListAsync(cancellationToken);
        var quizzes = await Quizzes(subjectId).Where(a => studentIds.Contains(a.StudentId))
            .GroupBy(a => new { a.StudentId, a.Status })
            .Select(g => new { g.Key.StudentId, g.Key.Status, Count = g.Count() }).ToListAsync(cancellationToken);
        var taskLookup = tasks.ToLookup(a => a.StudentId);
        var quizLookup = quizzes.ToLookup(a => a.StudentId);

        return studentIds.ToDictionary(id => id, id =>
        {
            var taskCounts = taskLookup[id].ToDictionary(a => a.Status, a => a.Count);
            var quizCounts = quizLookup[id].ToDictionary(a => a.Status, a => a.Count);
            int taskTotal = taskCounts.Values.Sum();
            int approved = taskCounts.GetValueOrDefault(StudentTaskStatus.Approved);
            int quizTotal = quizCounts.Values.Sum();
            int passed = quizCounts.GetValueOrDefault(QuizStatus.Passed);
            int failed = quizCounts.GetValueOrDefault(QuizStatus.Failed);
            return new ProgressSummaryDto(
                new(taskTotal, approved, taskCounts.GetValueOrDefault(StudentTaskStatus.Submitted),
                    taskCounts.GetValueOrDefault(StudentTaskStatus.Rejected),
                    taskCounts.GetValueOrDefault(StudentTaskStatus.NotSubmitted),
                    new Progression(approved, taskTotal).Percentage.Value),
                new(quizTotal, passed + failed, passed, failed,
                    new Progression(passed + failed, quizTotal).Percentage.Value));
        });
    }

    private IQueryable<DashboardTaskDto> TaskPreviews(IQueryable<AssignmentRow> assignments) =>
        assignments.Select(a => new DashboardTaskDto(a.AssignmentId, a.TaskId, a.Title, a.SubjectId,
            a.GroupId.HasValue ? "group" : "student", a.GroupId, a.Status.ToString(), a.AssignedAt, a.Deadline,
            context.Submissions.Where(s => s.TaskStudentId == a.AssignmentId || s.TaskGroupId == a.AssignmentId)
                .Select(s => (DateTimeOffset?)s.SubmittedAt).FirstOrDefault(),
            context.Feedback.Count(f => f.Submission.TaskStudentId == a.AssignmentId ||
                f.Submission.TaskGroupId == a.AssignmentId)));

    private IQueryable<FeedbackRow> FeedbackFor(IQueryable<AssignmentRow> assignments) =>
        from a in assignments
        join s in context.Submissions.AsNoTracking()
            on a.AssignmentId equals (s.TaskStudentId ?? s.TaskGroupId!.Value)
        join f in context.Feedback.AsNoTracking() on s.Id equals f.SubmissionId
        select new FeedbackRow
        {
            Id = f.Id, TaskId = a.TaskId, TaskTitle = a.Title, AssignmentId = a.AssignmentId,
            SubmissionId = s.Id, Text = f.Content, CreatedAt = f.CreatedAt,
            TeacherId = f.TeacherId, TeacherName = f.Teacher.Name
        };

    public async Task<StudentTaskProgressDto?> GetTaskAsync(Guid studentId, Guid taskId,
        CancellationToken cancellationToken)
    {
        var assignments = Assignments(null).Where(a => a.StudentId == studentId && a.TaskId == taskId);
        var previews = await TaskPreviews(assignments.OrderBy(a => a.AssignedAt).ThenBy(a => a.AssignmentId))
            .ToListAsync(cancellationToken);
        if (previews.Count == 0) return null;

        var task = await context.Tasks.AsNoTracking().Where(t => t.Id == taskId)
            .Select(t => new TaskDto(t.Id, t.SubjectId, t.Title, t.Description, t.Deadline, t.CreatedAt, t.CreatedByUserId))
            .SingleAsync(cancellationToken);
        var objectives = await context.TaskEducationalGoals.AsNoTracking().Where(o => o.TaskId == taskId)
            .OrderBy(o => o.EducationalGoalId)
            .Select(o => new TaskObjectiveDto(o.EducationalGoalId, o.EducationalGoal.SubjectId, o.EducationalGoal.Content))
            .ToListAsync(cancellationToken);
        Guid[] assignmentIds = previews.Select(a => a.AssignmentId).ToArray();
        var submissions = await context.Submissions.AsNoTracking()
            .Where(s => assignmentIds.Contains(s.TaskStudentId ?? s.TaskGroupId!.Value))
            .Select(s => new
            {
                s.Id, AssignmentId = s.TaskStudentId ?? s.TaskGroupId!.Value, s.Comment, s.SubmittedAt,
                s.SubmittedByUserId,
                File = context.SubmissionFiles.Where(f => f.SubmissionId == s.Id)
                    .Select(f => new SubmissionFileDto(f.Id, f.FileName.Value, f.UploadedAt)).FirstOrDefault()
            }).ToListAsync(cancellationToken);
        var feedback = await FeedbackFor(assignments).OrderBy(f => f.CreatedAt).ThenBy(f => f.Id)
            .ToListAsync(cancellationToken);
        var feedbackLookup = feedback.ToLookup(f => f.AssignmentId);
        var submissionLookup = submissions.ToDictionary(s => s.AssignmentId);

        return new(studentId, task, objectives, previews.Select(a =>
        {
            var s = submissionLookup.GetValueOrDefault(a.AssignmentId);
            return new TaskAssignmentProgressDto(a,
                s == null ? null : new SubmissionDto(s.Id, s.AssignmentId, s.Comment, s.SubmittedAt,
                    s.SubmittedByUserId, a.Status, s.File),
                feedbackLookup[a.AssignmentId].Select(f => new FeedbackDto(f.Id, studentId, taskId,
                    f.Text, f.CreatedAt, f.TeacherId, f.TeacherName, f.SubmissionId, f.AssignmentId)).ToList());
        }).ToList());
    }

    private sealed class FeedbackRow
    {
        public Guid Id { get; init; }
        public Guid TaskId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
        public Guid AssignmentId { get; init; }
        public Guid SubmissionId { get; init; }
        public string Text { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public Guid TeacherId { get; init; }
        public string TeacherName { get; init; } = string.Empty;
    }

    private sealed class AssignmentRow
    {
        public Guid StudentId { get; init; }
        public Guid AssignmentId { get; init; }
        public Guid TaskId { get; init; }
        public string Title { get; init; } = string.Empty;
        public Guid SubjectId { get; init; }
        public Guid? GroupId { get; init; }
        public StudentTaskStatus Status { get; init; }
        public DateTimeOffset AssignedAt { get; init; }
        public DateTimeOffset? Deadline { get; init; }
    }
}
