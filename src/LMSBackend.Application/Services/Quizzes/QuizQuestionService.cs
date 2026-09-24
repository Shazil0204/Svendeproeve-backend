using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Services.Quizzes;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizAnswerOptionRepository _quizAnswerOptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public QuizQuestionService(
        IQuizRepository quizRepository,
        IQuizQuestionRepository quizQuestionRepository,
        IQuizAnswerOptionRepository quizAnswerOptionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _quizRepository = quizRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _quizAnswerOptionRepository = quizAnswerOptionRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task AddQuestionAsync(
        Guid quizId,
        CreateQuizQuestionRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        EnsureQuizOwner(quiz, userId);

        List<CreateQuizAnswerOptionRequest> answerOptions =
            request.AnswerOptions.ToList();

        if (answerOptions.Count != 4)
        {
            throw new ValidationException(
                "A question must have exactly 4 answer options.");
        }

        int correctAnswerCount =
            answerOptions.Count(answerOption => answerOption.IsCorrect);

        if (correctAnswerCount != 1)
        {
            throw new ValidationException(
                "A question must have exactly one correct answer.");
        }

        QuizQuestion quizQuestion =
            new QuizQuestion(quizId, request.QuestionText);

        await _quizQuestionRepository.AddQuizQuestion(quizQuestion);

        foreach (CreateQuizAnswerOptionRequest answerOptionRequest
                 in answerOptions)
        {
            QuizAnswerOption answerOption = new QuizAnswerOption(
                quizQuestion.Id,
                answerOptionRequest.AnswerText,
                answerOptionRequest.IsCorrect);

            await _quizAnswerOptionRepository
                .AddQuizAnswerOption(answerOption);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateQuestionAsync(
        Guid questionId,
        UpdateQuizQuestionRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        QuizQuestion? question =
            await _quizQuestionRepository.GetQuizQuestionById(questionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        Quiz? quiz =
            await _quizRepository.GetQuizById(question.QuizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        EnsureQuizOwner(quiz, userId);

        bool hasStudentAnswers =
            await _quizQuestionRepository.HasStudentAnswers(questionId);

        if (hasStudentAnswers)
        {
            throw new ValidationException(
                "The question cannot be updated because a student has already answered it.");
        }

        question.Update(request.QuestionText);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<QuizQuestionResponse>>
        GetQuestionsByQuizIdAsync(Guid quizId)
    {
        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        IEnumerable<QuizQuestion> questions =
            await _quizQuestionRepository.GetQuizQuestionsByQuizId(quizId);

        List<QuizQuestionResponse> responses = [];

        foreach (QuizQuestion question in questions)
        {
            IEnumerable<QuizAnswerOption> answerOptions =
                await _quizAnswerOptionRepository
                    .GetQuizAnswerOptionsByQuestionId(question.Id);

            IEnumerable<QuizAnswerOptionResponse> optionResponses =
                answerOptions.Select(answerOption =>
                    new QuizAnswerOptionResponse(
                        answerOption.Id,
                        answerOption.QuizQuestionId,
                        answerOption.AnswerText,
                        answerOption.IsCorrect));

            responses.Add(
                new QuizQuestionResponse(
                    question.Id,
                    question.QuizId,
                    question.QuestionText,
                    optionResponses));
        }

        return responses;
    }

    public async Task DeleteQuestionAsync(Guid questionId)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        QuizQuestion? question =
            await _quizQuestionRepository.GetQuizQuestionById(questionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        Quiz? quiz =
            await _quizRepository.GetQuizById(question.QuizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        EnsureQuizOwner(quiz, userId);

        bool hasStudentAnswers =
            await _quizQuestionRepository.HasStudentAnswers(questionId);

        if (hasStudentAnswers)
        {
            throw new ValidationException(
                "The question cannot be deleted because a student has already answered it.");
        }

        IEnumerable<QuizAnswerOption> answerOptions =
            await _quizAnswerOptionRepository
                .GetQuizAnswerOptionsByQuestionId(questionId);

        foreach (QuizAnswerOption answerOption in answerOptions)
        {
            _quizAnswerOptionRepository
                .DeleteQuizAnswerOption(answerOption);
        }

        _quizQuestionRepository.DeleteQuizQuestion(question);

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

    private static void EnsureQuizOwner(
        Quiz quiz,
        Guid userId)
    {
        if (quiz.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot modify this quiz.");
        }
    }
}