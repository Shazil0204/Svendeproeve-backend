using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Tasks;
using LMSBackend.Application.DTOs.Tasks;
using LMSBackend.Application.Exceptions;
using TaskEntity = LMSBackend.Domain.Entities.Tasks.Task;
using TaskStudent = LMSBackend.Domain.Entities.Tasks.TaskStudent;
using TaskGroup = LMSBackend.Domain.Entities.Tasks.TaskGroup;

namespace LMSBackend.Application.Services.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public TaskService(
        ITaskRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        this._repository = repository;
        this._unitOfWork = unitOfWork;
        this._currentUserService = currentUserService;
    }
    public async Task<IReadOnlyList<TaskDto>> ListAsync(CancellationToken cancellationToken) =>
        (await _repository.GetAllAsync(cancellationToken)).Select(ToDto).ToList();

    public async Task<TaskDetailsDto> GetAsync(Guid taskId, CancellationToken cancellationToken)
    {
        TaskEntity task = await FindAsync(taskId, cancellationToken);
        IReadOnlyList<Domain.Entities.Tasks.TaskEducationalGoal> objectives = await _repository.GetObjectiveLinksAsync(taskId, cancellationToken);
        IReadOnlyList<TaskAssignmentDto> assignments = await _repository.GetAssignmentsAsync(taskId, null, null, cancellationToken);
        return new TaskDetailsDto(ToDto(task), objectives.Select(g =>
            new TaskObjectiveDto(g.EducationalGoalId, g.EducationalGoal.SubjectId, g.EducationalGoal.Content)).ToList(),
            assignments);
    }

    public async Task<TaskDto> CreateAsync(SaveTaskRequest request, CancellationToken cancellationToken)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var (title, description, deadline) = ValidateDetails(request);
        await RequireSubjectAsync(request.SubjectId, cancellationToken);
        TaskEntity task = new TaskEntity(request.SubjectId, userId, title, description, deadline);
        _repository.Add(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(task);
    }

    public async Task<TaskDto> UpdateAsync(Guid taskId, SaveTaskRequest request, CancellationToken cancellationToken)
    {
        TaskEntity task = await FindAsync(taskId, cancellationToken);
        var (title, description, deadline) = ValidateDetails(request);
        await RequireSubjectAsync(request.SubjectId, cancellationToken);
        if (task.SubjectId != request.SubjectId)
        {
            var objectives = await _repository.GetObjectiveLinksAsync(taskId, cancellationToken);
            if (objectives.Any(g => g.EducationalGoal.SubjectId != request.SubjectId))
                throw new ValidationException("Remove objectives from the previous subject before changing the task subject.");
        }

        task.UpdateDetails(title, description, deadline);
        task.ChangeSubject(request.SubjectId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(task);
    }

    public async Task DeleteAsync(Guid taskId, CancellationToken cancellationToken)
    {
        TaskEntity task = await FindAsync(taskId, cancellationToken);
        if (await _repository.HasSubmissionsAsync(taskId, cancellationToken))
            throw new ConflictException("A task with submissions cannot be deleted.");

        task.SoftDelete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskAssignmentDto>> AssignAsync(Guid taskId, AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        await FindAsync(taskId, cancellationToken);
        Guid[] studentIds = ValidateIds(request.StudentIds ?? [], "StudentIds");
        Guid[] groupIds = ValidateIds(request.GroupIds ?? [], "GroupIds");
        if (studentIds.Length + groupIds.Length == 0)
            throw new ValidationException("At least one student or group is required.");

        if (await _repository.CountActiveStudentsAsync(studentIds, cancellationToken) != studentIds.Length)
            throw new NotFoundException("One or more active students were not found.");
        if (await _repository.CountActiveGroupsAsync(groupIds, cancellationToken) != groupIds.Length)
            throw new NotFoundException("One or more groups were not found.");
        if (await _repository.AssignmentsExistAsync(taskId, studentIds, groupIds, cancellationToken))
            throw new ConflictException("The task is already assigned to one or more recipients.");

        IReadOnlyList<TaskStudent> students = studentIds.Select(id => new TaskStudent(taskId, id)).ToList();
        IReadOnlyList<TaskGroup> groups = groupIds.Select(id => new TaskGroup(taskId, id)).ToList();
        _repository.AddAssignments(students, groups);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        ISet<Guid> createdIds = students.Select(a => a.Id).Concat(groups.Select(a => a.Id)).ToHashSet();
        return (await _repository.GetAssignmentsAsync(taskId, null, null, cancellationToken))
            .Where(a => createdIds.Contains(a.AssignmentId)).ToList();
    }

    public async Task<IReadOnlyList<TaskObjectiveDto>> SetObjectivesAsync(Guid taskId,
        UpdateTaskObjectivesRequest request, CancellationToken cancellationToken)
    {
        TaskEntity task = await FindAsync(taskId, cancellationToken);
        Guid[] ids = ValidateIds(request.ObjectiveIds, "ObjectiveIds");
        var objectives = await _repository.GetObjectivesAsync(ids, cancellationToken);
        if (objectives.Count != ids.Length)
            throw new NotFoundException("One or more objectives were not found.");
        if (objectives.Any(g => g.SubjectId != task.SubjectId))
            throw new ValidationException("Objectives must belong to the task's subject.");

        var existing = await _repository.GetObjectiveLinksAsync(taskId, cancellationToken);
        _repository.ReplaceObjectives(taskId, existing, ids);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return objectives.OrderBy(g => g.Id).Select(g => new TaskObjectiveDto(g.Id, g.SubjectId, g.Content)).ToList();
    }

    public Task<IReadOnlyList<TaskAssignmentDto>> ListAssignmentsForStudentAsync(Guid studentId,
        CancellationToken cancellationToken) =>
        _repository.GetAssignmentsAsync(null, studentId, null, cancellationToken);

    public async Task<TaskAssignmentDto> GetAssignmentAsync(Guid assignmentId, Guid? studentId,
        CancellationToken cancellationToken) =>
        (await _repository.GetAssignmentsAsync(null, studentId, assignmentId, cancellationToken)).SingleOrDefault()
        ?? throw new NotFoundException("Assignment not found.");

    private async Task<TaskEntity> FindAsync(Guid taskId, CancellationToken cancellationToken) =>
        await _repository.GetByIdAsync(taskId, cancellationToken) ?? throw new NotFoundException("Task not found.");

    private async Task RequireSubjectAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        if (!await _repository.SubjectExistsAsync(subjectId, cancellationToken))
            throw new NotFoundException("Subject not found.");
    }

    private static (string Title, string Description, DateTimeOffset? Deadline) ValidateDetails(SaveTaskRequest request)
    {
        if (request.SubjectId == Guid.Empty)
            throw new ValidationException("SubjectId must be a non-empty ID.");
        string? title = request.Title?.Trim();
        if (string.IsNullOrEmpty(title) || title.Length > 200)
            throw new ValidationException("Title must contain 1 to 200 characters.");
        string description = request.Description ?? string.Empty;
        if (description.Length > 5000)
            throw new ValidationException("Description cannot exceed 5000 characters.");
        return (title, description, request.Deadline?.ToUniversalTime());
    }

    private static Guid[] ValidateIds(IReadOnlyCollection<Guid>? ids, string field)
    {
        if (ids is null || ids.Contains(Guid.Empty))
            throw new ValidationException($"{field} must be an array of non-empty IDs.");
        return ids.Distinct().ToArray();
    }

    private static TaskDto ToDto(TaskEntity task) =>
        new(task.Id, task.SubjectId, task.Title, task.Description, task.Deadline, task.CreatedAt, task.CreatedByUserId);
}
