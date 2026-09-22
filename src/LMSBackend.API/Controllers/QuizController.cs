using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.DTOs.Quizzes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz([FromBody] CreateQuizRequest quiz)
        {
            await _quizService.AddQuiz(quiz);
            return StatusCode(201);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(Guid quizId)
        {
            var quiz = await _quizService.GetQuizById(quizId);
            if (quiz == null)
            {
                return NotFound();
            }
            return Ok(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzes();
            return Ok(quizzes);
        }

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuiz(Guid quizId, [FromBody] UpdateQuizRequest updatedQuiz)
        {
            await _quizService.UpdateQuiz(quizId, updatedQuiz);
            return NoContent();
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            await _quizService.DeleteQuiz(quizId);
            return NoContent();
        }
    }
}
