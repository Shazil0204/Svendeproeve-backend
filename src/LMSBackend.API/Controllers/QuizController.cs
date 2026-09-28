using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Domain.Enums.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Controllers;

[ApiController]
[Route("api/v1/quiz")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IQuizQuestionService _quizQuestionService;
    private readonly IQuizAnswerOptionService _quizAnswerOptionService;
    private readonly IQuizStudentAnswerService _quizStudentAnswerService;

    public QuizController(
        IQuizService quizService,
        IQuizQuestionService quizQuestionService,
        IQuizAnswerOptionService quizAnswerOptionService,
        IQuizStudentAnswerService quizStudentAnswerService)
    {
        _quizService = quizService;
        _quizQuestionService = quizQuestionService;
        _quizAnswerOptionService = quizAnswerOptionService;
        _quizStudentAnswerService = quizStudentAnswerService;
    }

    // --------------------
    // Quiz
    // --------------------

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateQuiz(
        CreateQuizRequest request)
    {
        await _quizService.CreateQuizAsync(request);

        return NoContent();
    }

    [HttpPut("{quizId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateQuiz(
        Guid quizId,
        UpdateQuizRequest request)
    {
        await _quizService.UpdateQuizAsync(quizId, request);

        return NoContent();
    }

    [HttpGet("{quizId:guid}")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<ActionResult<QuizResponse>> GetQuiz(
        Guid quizId)
    {
        QuizResponse quiz =
            await _quizService.GetQuizByIdAsync(quizId);

        return Ok(quiz);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<QuizResponse>>> GetAllQuizzes()
    {
        IEnumerable<QuizResponse> quizzes =
            await _quizService.GetAllQuizzesAsync();

        return Ok(quizzes);
    }

    [Authorize(Roles = "Teacher, Student")]
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<QuizResponse>>> GetQuizzesByUser(
        Guid userId)
    {
        IEnumerable<QuizResponse> quizzes =
            await _quizService.GetQuizzesByUserIdAsync(userId);

        return Ok(quizzes);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student")]
    public async Task<ActionResult<IEnumerable<QuizResponse>>> GetQuizzesByStudent()
    {
        IEnumerable<QuizResponse> quizzes =
            await _quizService.GetQuizzesByStudentIdAsync();

        return Ok(quizzes);
    }

    [HttpDelete("{quizId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteQuiz(
        Guid quizId)
    {
        await _quizService.DeleteQuizAsync(quizId);

        return NoContent();
    }

    // --------------------
    // Questions
    // --------------------

    [HttpPost("{quizId:guid}/questions")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddQuestion(
        Guid quizId,
        CreateQuizQuestionRequest request)
    {
        await _quizQuestionService.AddQuestionAsync(
            quizId,
            request);

        return NoContent();
    }

    [HttpPut("questions/{questionId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateQuestion(
        Guid questionId,
        UpdateQuizQuestionRequest request)
    {
        await _quizQuestionService.UpdateQuestionAsync(
            questionId,
            request);

        return NoContent();
    }

    [HttpGet("{quizId:guid}/questions")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<QuizQuestionResponse>>>
        GetQuestions(Guid quizId)
    {
        IEnumerable<QuizQuestionResponse> questions =
            await _quizQuestionService
                .GetQuestionsByQuizIdAsync(quizId);

        return Ok(questions);
    }

    [HttpDelete("questions/{questionId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteQuestion(
        Guid questionId)
    {
        await _quizQuestionService.DeleteQuestionAsync(questionId);

        return NoContent();
    }

    // --------------------
    // Answer Options
    // --------------------

    [HttpPost("questions/{questionId:guid}/options")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddAnswerOption(
        Guid questionId,
        CreateQuizAnswerOptionRequest request)
    {
        await _quizAnswerOptionService.AddAnswerOptionAsync(
            questionId,
            request);

        return NoContent();
    }

    [HttpPut("options/{answerOptionId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateAnswerOption(
        Guid answerOptionId,
        UpdateQuizAnswerOptionRequest request)
    {
        await _quizAnswerOptionService.UpdateAnswerOptionAsync(
            answerOptionId,
            request);

        return NoContent();
    }

    [HttpGet("questions/{questionId:guid}/options")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<QuizAnswerOptionResponse>>>
        GetAnswerOptions(Guid questionId)
    {
        IEnumerable<QuizAnswerOptionResponse> options =
            await _quizAnswerOptionService
                .GetAnswerOptionsByQuestionIdAsync(questionId);

        return Ok(options);
    }

    [HttpDelete("options/{answerOptionId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteAnswerOption(
        Guid answerOptionId)
    {
        await _quizAnswerOptionService
            .DeleteAnswerOptionAsync(answerOptionId);

        return NoContent();
    }

    // --------------------
    // Quiz Students
    // --------------------

    [HttpPost("{quizId:guid}/students")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddStudent(
        Guid quizId,
        AddQuizStudentRequest request)
    {
        await _quizStudentAnswerService.AddStudentToQuizAsync(
            quizId,
            request);

        return NoContent();
    }

    [HttpDelete("{quizId:guid}/students/{studentId:guid}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RemoveStudent(
        Guid quizId,
        Guid studentId)
    {
        await _quizStudentAnswerService.RemoveStudentFromQuizAsync(
            quizId,
            studentId);

        return NoContent();
    }

    [HttpGet("{quizId:guid}/students")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<QuizStudentResponse>>>
        GetQuizStudents(Guid quizId)
    {
        IEnumerable<QuizStudentResponse> students =
            await _quizStudentAnswerService
                .GetQuizStudentsAsync(quizId);

        return Ok(students);
    }

    // --------------------
    // Student Quiz
    // --------------------

    [HttpGet("{quizId:guid}/students/{studentId:guid}/review")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<ActionResult<QuizReviewResponse>> GetQuizReview(
        Guid quizId,
        Guid studentId)
    {
        QuizReviewResponse review =
            await _quizStudentAnswerService.GetQuizReviewAsync(
                quizId,
                studentId);

        return Ok(review);
    }

    [HttpGet("{quizId:guid}/student-status")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<QuizStudentResponse>> GetCurrentStudentQuizStatus(
        Guid quizId)
    {
        QuizStudentResponse status =
            await _quizStudentAnswerService
                .GetCurrentStudentQuizStatusAsync(quizId);

        return Ok(status);
    }

    [HttpGet("{quizId:guid}/students/{studentId:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<StudentQuizResponse>> GetStudentQuiz(
        Guid quizId,
        Guid studentId)
    {
        StudentQuizResponse quiz =
            await _quizStudentAnswerService.GetStudentQuizAsync(
                quizId,
                studentId);

        return Ok(quiz);
    }

    [HttpPost("{quizId:guid}/students/{studentId:guid}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitQuiz(
        Guid quizId,
        Guid studentId,
        SubmitQuizRequest request)
    {
        await _quizStudentAnswerService.SubmitQuizAsync(
            quizId,
            studentId,
            request);

        return NoContent();
    }

    [HttpGet("{quizId:guid}/students/{studentId:guid}/answers")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<ActionResult<IEnumerable<QuizAnswerResponse>>>
        GetStudentAnswers(
            Guid quizId,
            Guid studentId)
    {
        IEnumerable<QuizAnswerResponse> answers =
            await _quizStudentAnswerService
                .GetStudentAnswersAsync(
                    quizId,
                    studentId);

        return Ok(answers);
    }
}