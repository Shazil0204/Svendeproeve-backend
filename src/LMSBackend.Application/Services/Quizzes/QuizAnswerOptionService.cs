using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Quizzes;

namespace LMSBackend.Application.Services.Quizzes;

public class QuizAnswerOptionService : IQuizAnswerOptionService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizAnswerOptionRepository _quizAnswerOptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public QuizAnswerOptionService(
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

    public async Task AddAnswerOptionAsync(
        Guid questionId,
        CreateQuizAnswerOptionRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        QuizQuestion? question =
            await _quizQuestionRepository.GetQuizQuestionById(questionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        Quiz quiz = await GetOwnedQuizAsync(question.QuizId, userId);

        bool hasStudentAnswers =
            await _quizQuestionRepository.HasStudentAnswers(questionId);

        if (hasStudentAnswers)
        {
            throw new ValidationException(
                "Answer options cannot be changed after a student has answered the question.");
        }

        int optionCount =
            await _quizAnswerOptionRepository
                .GetAnswerOptionCountByQuestionId(questionId);

        if (optionCount >= 4)
        {
            throw new ValidationException(
                "A question cannot have more than 4 answer options.");
        }

        if (request.IsCorrect)
        {
            bool hasCorrectAnswer =
                await _quizAnswerOptionRepository
                    .HasCorrectAnswer(questionId);

            if (hasCorrectAnswer)
            {
                throw new ValidationException(
                    "A question can only have one correct answer.");
            }
        }

        QuizAnswerOption answerOption = new(
            questionId,
            request.AnswerText,
            request.IsCorrect);

        await _quizAnswerOptionRepository
            .AddQuizAnswerOption(answerOption);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAnswerOptionAsync(
        Guid answerOptionId,
        UpdateQuizAnswerOptionRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        QuizAnswerOption? answerOption =
            await _quizAnswerOptionRepository
                .GetQuizAnswerOptionById(answerOptionId);

        if (answerOption is null)
        {
            throw new NotFoundException("Answer option not found.");
        }

        QuizQuestion? question =
            await _quizQuestionRepository
                .GetQuizQuestionById(answerOption.QuizQuestionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        Quiz quiz = await GetOwnedQuizAsync(question.QuizId, userId);

        bool hasStudentAnswers =
            await _quizQuestionRepository.HasStudentAnswers(question.Id);

        if (hasStudentAnswers)
        {
            throw new ValidationException(
                "Answer options cannot be changed after a student has answered the question.");
        }

        if (request.IsCorrect && !answerOption.IsCorrect)
        {
            bool hasCorrectAnswer =
                await _quizAnswerOptionRepository
                    .HasCorrectAnswer(question.Id);

            if (hasCorrectAnswer)
            {
                throw new ValidationException(
                    "A question can only have one correct answer.");
            }
        }

        answerOption.Update(
            request.AnswerText,
            request.IsCorrect);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<QuizAnswerOptionResponse>>
        GetAnswerOptionsByQuestionIdAsync(Guid questionId)
    {
        QuizQuestion? question =
            await _quizQuestionRepository.GetQuizQuestionById(questionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        IEnumerable<QuizAnswerOption> answerOptions =
            await _quizAnswerOptionRepository
                .GetQuizAnswerOptionsByQuestionId(questionId);

        return answerOptions.Select(
            answerOption => new QuizAnswerOptionResponse(
                answerOption.Id,
                answerOption.QuizQuestionId,
                answerOption.AnswerText,
                answerOption.IsCorrect));
    }

    public async Task DeleteAnswerOptionAsync(Guid answerOptionId)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        QuizAnswerOption? answerOption =
            await _quizAnswerOptionRepository
                .GetQuizAnswerOptionById(answerOptionId);

        if (answerOption is null)
        {
            throw new NotFoundException("Answer option not found.");
        }

        QuizQuestion? question =
            await _quizQuestionRepository
                .GetQuizQuestionById(answerOption.QuizQuestionId);

        if (question is null)
        {
            throw new NotFoundException("Question not found.");
        }

        Quiz quiz = await GetOwnedQuizAsync(question.QuizId, userId);

        bool hasStudentAnswers =
            await _quizQuestionRepository.HasStudentAnswers(question.Id);

        if (hasStudentAnswers)
        {
            throw new ValidationException(
                "Answer options cannot be changed after a student has answered the question.");
        }

        _quizAnswerOptionRepository.DeleteQuizAnswerOption(answerOption);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Quiz> GetOwnedQuizAsync(
        Guid quizId,
        Guid userId)
    {
        Quiz? quiz =
            await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        if (quiz.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot modify this quiz.");
        }

        return quiz;
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
}