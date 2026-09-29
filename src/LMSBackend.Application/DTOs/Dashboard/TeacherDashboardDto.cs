using LMSBackend.Application.DTOs.Subjects;
using LMSBackend.Application.DTOs.Tasks;

namespace LMSBackend.Application.DTOs.Dashboard;

public sealed record TeacherDashboardDto(Guid TeacherId, string TeacherName, string Email,
    PageDto<SubjectResponseDTO> Subjects, PageDto<TaskDto> Tasks,
    PageDto<TeacherDashboardAssignmentDto> Assignments);
