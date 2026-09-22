using LMSBackend.Application.Abstractions.EducationalGoals;
using LMSBackend.Application.DTOs.EducationalGoals;
using LMSBackend.Application.Services.EducationalGoals;
using LMSBackend.Domain.Entities.Subjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IEduGoalService _eduGoalService;

        public GoalController(IEduGoalService eduGoalService)
        {
            _eduGoalService = eduGoalService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllEduGoals()
        {
            var eduGoals = await _eduGoalService.GetAllEduGoals();
            return Ok(eduGoals);
        }

        [HttpPost]
        public async Task<IActionResult> AddEduGoal([FromBody] CreateEduGoalRequest eduGoal)
        {
            await _eduGoalService.AddEduGoal(eduGoal);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEduGoalContent(Guid id, [FromBody] UpdateEduGoalRequest updateRequest)
        {
            await _eduGoalService.UpdateEduGoalUpdateContent(id, updateRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEduGoal(Guid id)
        {
            await _eduGoalService.DeleteEduGoal(id);
            return NoContent();
        }
    }
}
