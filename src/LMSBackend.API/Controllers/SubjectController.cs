using LMSBackend.Application.Abstractions.Subjects;
using LMSBackend.Application.DTOs.Subjects;
using LMSBackend.Domain.Entities.Subjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject([FromBody] CreateSubject createSubject)
        {
            await _subjectService.AddSubject(createSubject);
            return StatusCode(201);
        }

        [HttpGet("{subjectId}")]
        public async Task<IActionResult> GetSubjectById(Guid subjectId)
        {
            SubjectResponseDTO? subject = await _subjectService.GetSubjectById(subjectId);
            if (subject == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetSubjectById), new { subjectId = subject.Id }, subject);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            IEnumerable<SubjectResponseDTO> subjects = await _subjectService.GetAllSubjects();
            return Ok(subjects);
        }

        [HttpPut("{subjectId}")]
        public async Task<IActionResult> UpdateSubject(Guid subjectId, [FromBody] UpdateSubject updateSubject)
        {
            await _subjectService.UpdateSubject(updateSubject, subjectId);
            return NoContent();
        }

        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> SoftDeleteSubject(Guid subjectId)
        {
            await _subjectService.SoftDeleteSubject(subjectId);
            return NoContent();
        }
    }
}
