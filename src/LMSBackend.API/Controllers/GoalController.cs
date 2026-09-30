using LMSBackend.Application.Abstractions.EducationalGoals;
using LMSBackend.Application.DTOs.EducationalGoals;
using LMSBackend.Application.Services.EducationalGoals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IEduGoalService _eduGoalService;

        public GoalController(IEduGoalService eduGoalService)
        {
            _eduGoalService = eduGoalService;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEduGoalById(Guid id)
        {
            EduGoalResponse? eduGoal = await _eduGoalService.GetEduGoalById(id);
            if (eduGoal == null)
            {
                return NotFound();
            }
            return Ok(eduGoal);
        }

        [Authorize(Roles = "Teacher, Student")]
        [HttpGet("subjectid/{subjectId}")]
        public async Task<IActionResult> GetEduGoalsBySubjectId(Guid subjectId)
        {
            List<EduGoalResponse> eduGoals = await _eduGoalService.GetEduGoalsBySubjectId(subjectId);
            return Ok(eduGoals);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> GetAllEduGoals()
        {
            IEnumerable<EduGoalResponse>? eduGoals = await _eduGoalService.GetAllEduGoals();
            return Ok(eduGoals);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddEduGoal([FromBody] CreateEduGoalRequest eduGoal)
        {
            await _eduGoalService.AddEduGoal(eduGoal);
            return StatusCode(201);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEduGoalContent(Guid id, [FromBody] UpdateEduGoalRequest updateRequest)
        {
            await _eduGoalService.UpdateEduGoalUpdateContent(id, updateRequest);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEduGoal(Guid id)
        {
            await _eduGoalService.DeleteEduGoal(id);
            return NoContent();
        }

        [Authorize(Roles = "Teacher, Student")]
        [HttpGet("single/subjectid/{subjectId}")]
        public async Task<IActionResult> GetEduGoalBySubjectId(Guid subjectId)
        {
            IReadOnlyList<EduGoalResponse> eduGoals = await _eduGoalService.GetEduGoalBySubjectId(subjectId);
            return Ok(eduGoals);
        }
    }
}
