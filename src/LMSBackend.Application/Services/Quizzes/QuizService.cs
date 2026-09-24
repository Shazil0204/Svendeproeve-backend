using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.Entities.Subjects;
using LMSBackend.Domain.ValueObjects.Progression;

namespace LMSBackend.Application.Services.Quizzes;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizAnswerOptionRepository _quizAnswerOptionRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public QuizService(
        IQuizRepository quizRepository,
        IQuizQuestionRepository quizQuestionRepository,
        IQuizAnswerOptionRepository quizAnswerOptionRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _quizRepository = quizRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _quizAnswerOptionRepository = quizAnswerOptionRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task CreateQuizAsync(CreateQuizRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        Subject? subject =
            await _subjectRepository.GetSubjectById(request.SubjectId) ?? throw new NotFoundException("Subject not found.");
        
        List<CreateQuizQuestionRequest> questions =
            request.Questions.ToList();

        foreach (CreateQuizQuestionRequest question in questions)
        {
            List<CreateQuizAnswerOptionRequest> answerOptions =
                question.AnswerOptions.ToList();

            if (answerOptions.Count != 4)
            {
                throw new ValidationException(
                    "Each question must have exactly 4 answer options.");
            }

            int correctAnswerCount =
                answerOptions.Count(answerOption => answerOption.IsCorrect);

            if (correctAnswerCount != 1)
            {
                throw new ValidationException(
                    "Each question must have exactly one correct answer.");
            }
        }

        Percentage passingPercentage = new(request.PassingPercentage);

        Quiz quiz = new(
            request.SubjectId,
            userId,
            request.Title,
            request.Description,
            passingPercentage);

        await _quizRepository.AddQuiz(quiz);

        foreach (CreateQuizQuestionRequest questionRequest in questions)
        {
            QuizQuestion question = new QuizQuestion(
                quiz.Id,
                questionRequest.QuestionText);

            await _quizQuestionRepository.AddQuizQuestion(question);

            foreach (CreateQuizAnswerOptionRequest answerOptionRequest
                     in questionRequest.AnswerOptions)
            {
                QuizAnswerOption answerOption = new QuizAnswerOption(
                    question.Id,
                    answerOptionRequest.AnswerText,
                    answerOptionRequest.IsCorrect);

                await _quizAnswerOptionRepository
                    .AddQuizAnswerOption(answerOption);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateQuizAsync(
        Guid quizId,
        UpdateQuizRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        Quiz? quiz =
            await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        if (quiz.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot update this quiz.");
        }

        Percentage passingPercentage =
            new Percentage(request.PassingPercentage);

        quiz.Update(
            request.Title,
            request.Description,
            passingPercentage);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<QuizResponse> GetQuizByIdAsync(Guid quizId)
    {
        Quiz? quiz =
            await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        return MapQuiz(quiz);
    }

    public async Task<IEnumerable<QuizResponse>> GetQuizzesByUserIdAsync(
        Guid userId)
    {
        IEnumerable<Quiz> quizzes =
            await _quizRepository.GetQuizzesByUserId(userId);

        return quizzes.Select(MapQuiz);
    }

    public async Task<IEnumerable<QuizResponse>> GetAllQuizzesAsync()
    {
        IEnumerable<Quiz> quizzes =
            await _quizRepository.GetAllQuizzes();

        return quizzes.Select(MapQuiz);
    }

    public async Task DeleteQuizAsync(Guid quizId)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        Quiz? quiz =
            await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        if (quiz.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot delete this quiz.");
        }

        quiz.SoftDelete();

        await _unitOfWork.SaveChangesAsync();
    }

    private Guid GetCurrentUserId()
    {
        return _currentUserService.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");
    }

    private void EnsureTeacher()
    {
        if (_currentUserService.Role != "Teacher")
        {
            throw new UnauthorizedAccessException(
                "Only teachers can perform this action.");
        }
    }

    private static QuizResponse MapQuiz(Quiz quiz)
    {
        return new QuizResponse(
            quiz.Id,
            quiz.SubjectId,
            quiz.CreatedByUserId,
            quiz.Title,
            quiz.Description,
            quiz.PassingPercentage.Value,
            quiz.CreatedAt);
    }
}