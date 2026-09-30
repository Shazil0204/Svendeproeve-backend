using LMSBackend.Application.Abstractions.Subjects;
using LMSBackend.Application.DTOs.Subjects;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddSubject([FromBody] CreateSubject createSubject)
        {
            await _subjectService.AddSubject(createSubject);
            return StatusCode(201);
        }

        [Authorize(Roles = "Teacher")]
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

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            IEnumerable<SubjectResponseDTO> subjects = await _subjectService.GetAllSubjects();
            return Ok(subjects);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("{subjectId}")]
        public async Task<IActionResult> UpdateSubject(Guid subjectId, [FromBody] UpdateSubject updateSubject)
        {
            await _subjectService.UpdateSubject(updateSubject, subjectId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> SoftDeleteSubject(Guid subjectId)
        {
            await _subjectService.SoftDeleteSubject(subjectId);
            return NoContent();
        }
        
        [Authorize(Roles = "Teacher, Student")]
        [HttpGet("getallmysubjects")]
        public async Task<IActionResult> GetAllSubjectsByUserId()
        {
            IEnumerable<SubjectResponseDTO> subjects = await _subjectService.GetAllSubjectsByUserId();
            return Ok(subjects);
        }
    }
}
