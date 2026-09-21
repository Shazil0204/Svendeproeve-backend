using LMSBackend.Application.DTOs.Groups;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Groups;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Groups;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Domain.Enums.Auditing;

namespace LMSBackend.Application.Services.Groups;

public sealed class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;


    public GroupService(IGroupRepository repository, IUnitOfWork unitOfWork, IAuditService auditService, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GroupDto>> ListAsync(CancellationToken cancellationToken) =>
        (await _repository.GetAllAsync(cancellationToken))
            .Select(group => new GroupDto(group.Id, group.Name, group.CreatedAt)).ToList();

    public async Task<GroupDetailsDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        StudentGroup group = await FindAsync(id, cancellationToken);
        List<GroupStudentDto> students = (await _repository.GetMembershipsAsync(id, cancellationToken))
            .Select(membership => new GroupStudentDto(
                membership.StudentId, membership.Student.Name, membership.AddedAt)).ToList();
        return new(group.Id, group.Name, group.CreatedAt, students);
    }

    public async Task<IReadOnlyList<GroupDto>> ListForStudentAsync(Guid studentId, CancellationToken cancellationToken) =>
        (await _repository.GetForStudentAsync(studentId, cancellationToken))
            .Select(group => new GroupDto(group.Id, group.Name, group.CreatedAt)).ToList();

    public async Task<GroupDetailsDto> GetForStudentAsync(Guid id, Guid studentId, CancellationToken cancellationToken)
    {
        if (await _repository.GetMembershipAsync(id, studentId, cancellationToken) is null)
            throw new NotFoundException("Group not found.");

        return await GetAsync(id, cancellationToken);
    }

    public async Task<GroupDto> CreateAsync(string name, CancellationToken cancellationToken)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        name = ValidateName(name);
        await CheckNameAsync(name, null, cancellationToken);
        StudentGroup group = new StudentGroup(name);
        await _repository.AddAsync(group, cancellationToken);
        await _auditService.LogAsync(
            userId,
            AuditAction.Created,
            "Group created",
            nameof(StudentGroup),
            group.Id,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new GroupDto(group.Id, group.Name, group.CreatedAt);
    }

    public async Task RenameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        StudentGroup group = await FindAsync(id, cancellationToken);
        name = ValidateName(name);
        await CheckNameAsync(name, id, cancellationToken);
        group.UpdateName(name);
        await _repository.UpdateAsync(group, cancellationToken);
        await _auditService.LogAsync(
            userId,
            AuditAction.Updated,
            "Group renamed",
            nameof(StudentGroup),
            group.Id,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        StudentGroup group = await FindAsync(id, cancellationToken);
        group.SoftDelete();
        await _repository.UpdateAsync(group, cancellationToken);
        await _auditService.LogAsync(
            userId,
            AuditAction.Deleted,
            "Group deleted",
            nameof(StudentGroup),
            group.Id,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddStudentsAsync(Guid id, IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken)
    {
        Guid[] ids = ValidateStudentIds(studentIds);
        await FindAsync(id, cancellationToken);
        var students = await _repository.GetStudentsAsync(ids, cancellationToken);
        if (students.Count != ids.Length)
            throw new NotFoundException("One or more students were not found.");
        if (students.Any(student => student.Role != UserRole.Student || !student.IsActive))
            throw new ValidationException("Only active students can join a group.");
        if ((await _repository.GetMembershipsAsync(id, ids, cancellationToken)).Count > 0)
            throw new ConflictException("One or more students are already members.");

        await _repository.AddMembershipsAsync(ids.Select(studentId => new GroupMembership(id, studentId)), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveStudentsAsync(Guid id, IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken)
    {
        Guid[] ids = ValidateStudentIds(studentIds);
        await FindAsync(id, cancellationToken);
        var memberships = await _repository.GetMembershipsAsync(id, ids, cancellationToken);
        if (memberships.Count != ids.Length)
            throw new NotFoundException("One or more memberships were not found.");

        await _repository.RemoveMembershipsAsync(memberships, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static Guid[] ValidateStudentIds(IReadOnlyCollection<Guid>? studentIds)
    {
        if (studentIds is null || studentIds.Count == 0 || studentIds.Contains(Guid.Empty))
            throw new ValidationException("StudentIds must contain at least one non-empty student ID and no empty IDs.");
        return studentIds.Distinct().ToArray();
    }

    private async Task<StudentGroup> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await _repository.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException("Group not found.");

    private static string ValidateName(string name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > 200)
            throw new ValidationException("Group name must contain 1 to 200 characters.");
        return trimmed;
    }

    private async Task CheckNameAsync(string name, Guid? exceptId, CancellationToken cancellationToken)
    {
        if (await _repository.NameExistsAsync(name, exceptId, cancellationToken))
            throw new ConflictException("A group with this name already exists.");
    }

}
