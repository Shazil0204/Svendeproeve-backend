using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.ValueObjects.Progression;

namespace LMSBackend.Application.Services.Quizzes;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAuditService auditService)
    {
        _quizRepository = quizRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    # region QUIZ SERVICE METHODS
    public async Task AddQuiz(CreateQuizRequest quiz)
    {
        Guid createdByUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        Percentage percentage = new Percentage(quiz.PassingPercentage);
        Quiz newQuiz = new(quiz.SubjectId, createdByUserId, quiz.Title, quiz.Description, percentage);
        await _quizRepository.AddQuiz(newQuiz);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<QuizResponse?> GetQuizById(Guid quizId)
    {
        Quiz? quiz = await _quizRepository.GetQuizById(quizId);
        if (quiz == null) throw new NotFoundException("Quiz not found.");

        return new QuizResponse
        (
            quiz.Id,
            quiz.SubjectId,
            quiz.CreatedByUserId,
            quiz.Title,
            quiz.Description,
            quiz.PassingPercentage.Value,
            quiz.CreatedAt
        );
    }

    public async Task<IEnumerable<QuizResponse>> GetAllQuizzes()
    {
        IEnumerable<Quiz> quizzes = await _quizRepository.GetAllQuizzes();
        return quizzes.Select(quiz => new QuizResponse
        (
            quiz.Id,
            quiz.SubjectId,
            quiz.CreatedByUserId,
            quiz.Title,
            quiz.Description,
            decimal.Parse(quiz.PassingPercentage.Value.ToString()),
            quiz.CreatedAt
        ));
    }

    public async Task UpdateQuiz(Guid quizId, UpdateQuizRequest updatedQuiz)
    {
        Quiz quiz = await _quizRepository.GetQuizById(quizId) ?? throw new NotFoundException("Quiz not found.");
        if (updatedQuiz.Title == null && updatedQuiz.Description == null && updatedQuiz.PassingPercentage == null)
        {
            throw new ValidationException("At least one field must be provided for update.");
        }
        int passingPercentageValue = updatedQuiz.PassingPercentage ?? (int)quiz.PassingPercentage.Value;
        string titleValue = updatedQuiz.Title ?? quiz.Title;
        string descriptionValue = updatedQuiz.Description ?? quiz.Description;
        Percentage percentage = new Percentage(passingPercentageValue);
        quiz.Update(titleValue, descriptionValue, percentage);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteQuiz(Guid quizId)
    {
        Quiz quiz = await _quizRepository.GetQuizById(quizId) ?? throw new NotFoundException("Quiz not found.");
        if (quiz == null) return;
        quiz.SoftDelete();
        await _unitOfWork.SaveChangesAsync();
    }
    # endregion
}
