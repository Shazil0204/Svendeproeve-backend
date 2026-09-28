using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.DTOs.Quizzes;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Quizzes;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Progression;

namespace LMSBackend.Application.Services.Quizzes;

public class QuizStudentAnswerService : IQuizStudentAnswerService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizAnswerOptionRepository _quizAnswerOptionRepository;
    private readonly IQuizStudentAnswerRepository _quizStudentAnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public QuizStudentAnswerService(
        IQuizRepository quizRepository,
        IQuizQuestionRepository quizQuestionRepository,
        IQuizAnswerOptionRepository quizAnswerOptionRepository,
        IQuizStudentAnswerRepository quizStudentAnswerRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _quizRepository = quizRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _quizAnswerOptionRepository = quizAnswerOptionRepository;
        _quizStudentAnswerRepository = quizStudentAnswerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task AddStudentToQuizAsync(
        Guid quizId,
        AddQuizStudentRequest request)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        Quiz quiz = await GetOwnedQuizAsync(quizId, userId);

        User? student =
            await _userRepository.GetByIdAsync(request.StudentId);

        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        if (student.Role != UserRole.Student)
        {
            throw new ValidationException(
                "Only students can be assigned to a quiz.");
        }

        QuizStudent? existingAssignment =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quiz.Id,
                student.Id);

        if (existingAssignment is not null)
        {
            throw new ValidationException(
                "Student is already assigned to this quiz.");
        }

        QuizStudent quizStudent =
            new QuizStudent(quiz.Id, student.Id);

        await _quizStudentAnswerRepository.AddQuizStudent(quizStudent);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveStudentFromQuizAsync(
        Guid quizId,
        Guid studentId)
    {
        Guid userId = GetCurrentUserId();

        EnsureTeacher();

        await GetOwnedQuizAsync(quizId, userId);

        QuizStudent? quizStudent =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quizId,
                studentId);

        if (quizStudent is null)
        {
            throw new NotFoundException(
                "Student is not assigned to this quiz.");
        }

        IEnumerable<QuizAnswer> answers =
            await _quizStudentAnswerRepository
                .GetStudentAnswersByQuizStudentId(quizStudent.Id);

        if (answers.Any())
        {
            throw new ValidationException(
                "A student who has submitted the quiz cannot be removed.");
        }

        _quizStudentAnswerRepository.DeleteQuizStudent(quizStudent);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<QuizStudentResponse>>
        GetQuizStudentsAsync(Guid quizId)
    {
        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        IEnumerable<QuizStudent> quizStudents =
            await _quizStudentAnswerRepository
                .GetQuizStudentsByQuizId(quizId);

        return quizStudents.Select(
            quizStudent => new QuizStudentResponse(
                quizStudent.Id,
                quizStudent.QuizId,
                quizStudent.StudentId,
                quizStudent.AssignedAt,
                quizStudent.ScorePercentage?.Value,
                quizStudent.Passed,
                quizStudent.CompletedAt,
                quizStudent.Status.ToString()));
    }

    public async Task<QuizStudentResponse> GetCurrentStudentQuizStatusAsync(
        Guid quizId)
    {
        EnsureStudent();

        Guid studentId = GetCurrentUserId();
        QuizStudent? quizStudent =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quizId,
                studentId);

        if (quizStudent is null)
        {
            throw new NotFoundException("Student is not assigned to this quiz.");
        }

        return new QuizStudentResponse(
            quizStudent.Id,
            quizStudent.QuizId,
            quizStudent.StudentId,
            quizStudent.AssignedAt,
            quizStudent.ScorePercentage?.Value,
            quizStudent.Passed,
            quizStudent.CompletedAt,
            quizStudent.Status.ToString());
    }

    public async Task<StudentQuizResponse> GetStudentQuizAsync(
        Guid quizId,
        Guid studentId)
    {
        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        QuizStudent? quizStudent =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quizId,
                studentId);

        if (quizStudent is null)
        {
            throw new UnauthorizedAccessException(
                "Student is not assigned to this quiz.");
        }

        IEnumerable<QuizQuestion> questions =
            await _quizQuestionRepository
                .GetQuizQuestionsByQuizId(quizId);

        List<StudentQuizQuestionResponse> questionResponses = [];

        foreach (QuizQuestion question in questions)
        {
            IEnumerable<QuizAnswerOption> options =
                await _quizAnswerOptionRepository
                    .GetQuizAnswerOptionsByQuestionId(question.Id);

            IEnumerable<StudentQuizAnswerOptionResponse> optionResponses =
                options.Select(
                    option => new StudentQuizAnswerOptionResponse(
                        option.Id,
                        option.AnswerText));

            questionResponses.Add(
                new StudentQuizQuestionResponse(
                    question.Id,
                    question.QuestionText,
                    optionResponses,
                    null));
        }

        return new StudentQuizResponse(
            quiz.Id,
            quiz.Title,
            quiz.Description,
            questionResponses);
    }

    public async Task SubmitQuizAsync(
        Guid quizId,
        Guid studentId,
        SubmitQuizRequest request)
    {
        Guid currentUserId = GetCurrentUserId();

        EnsureStudent();

        if (currentUserId != studentId)
        {
            throw new UnauthorizedAccessException(
                "You can only submit your own quiz.");
        }

        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz is null)
        {
            throw new NotFoundException("Quiz not found.");
        }

        QuizStudent? quizStudent =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quizId,
                studentId);

        if (quizStudent is null)
        {
            throw new UnauthorizedAccessException(
                "Student is not assigned to this quiz.");
        }

        IEnumerable<QuizAnswer> existingAnswers =
            await _quizStudentAnswerRepository
                .GetStudentAnswersByQuizStudentId(quizStudent.Id);

        if (existingAnswers.Any())
        {
            throw new ValidationException(
                "This quiz has already been submitted.");
        }

        List<QuizQuestion> questions =
            (await _quizQuestionRepository
                .GetQuizQuestionsByQuizId(quizId))
            .ToList();

        List<SubmitQuizAnswerRequest> submittedAnswers =
            request.Answers.ToList();

        if (submittedAnswers.Count != questions.Count)
        {
            throw new ValidationException(
                "Every quiz question must be answered.");
        }

        bool hasDuplicateQuestions =
            submittedAnswers
                .GroupBy(answer => answer.QuizQuestionId)
                .Any(group => group.Count() > 1);

        if (hasDuplicateQuestions)
        {
            throw new ValidationException(
                "A question cannot be answered more than once.");
        }

        int correctAnswers = 0;

        List<QuizAnswer> answersToAdd = [];

        foreach (QuizQuestion question in questions)
        {
            SubmitQuizAnswerRequest? submittedAnswer =
                submittedAnswers.FirstOrDefault(
                    answer => answer.QuizQuestionId == question.Id);

            if (submittedAnswer is null)
            {
                throw new ValidationException(
                    "Every quiz question must be answered.");
            }

            List<QuizAnswerOption> options =
                (await _quizAnswerOptionRepository
                    .GetQuizAnswerOptionsByQuestionId(question.Id))
                .ToList();

            if (options.Count != 4 ||
                options.Count(option => option.IsCorrect) != 1)
            {
                throw new ValidationException(
                    "The quiz contains an invalid question.");
            }

            QuizAnswerOption? selectedOption =
                options.FirstOrDefault(
                    option =>
                        option.Id == submittedAnswer.SelectedAnswerOptionId);

            if (selectedOption is null)
            {
                throw new ValidationException(
                    "Selected answer option does not belong to the question.");
            }

            if (selectedOption.IsCorrect)
            {
                correctAnswers++;
            }

            QuizAnswer quizAnswer = new(
                quizStudent.Id,
                question.Id,
                selectedOption.Id);

            answersToAdd.Add(quizAnswer);
        }

        decimal score =
            questions.Count == 0
                ? 0
                : (decimal)correctAnswers / questions.Count * 100;

        Percentage scorePercentage = new Percentage(score);

        bool passed =
            scorePercentage.Value >= quiz.PassingPercentage.Value;

        foreach (QuizAnswer answer in answersToAdd)
        {
            await _quizStudentAnswerRepository
                .AddStudentQuizAnswer(answer);
        }

        QuizStatus status = passed
            ? QuizStatus.Passed
            : QuizStatus.Failed;

        quizStudent.CompleteQuiz(
            scorePercentage,
            passed,
            status);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<QuizAnswerResponse>>
        GetStudentAnswersAsync(
            Guid quizId,
            Guid studentId)
    {
        QuizStudent? quizStudent =
            await _quizStudentAnswerRepository.GetQuizStudent(
                quizId,
                studentId);

        if (quizStudent is null)
        {
            throw new NotFoundException(
                "Student is not assigned to this quiz.");
        }

        IEnumerable<QuizAnswer> answers =
            await _quizStudentAnswerRepository
                .GetStudentAnswersByQuizStudentId(quizStudent.Id);

        return answers.Select(
            answer => new QuizAnswerResponse(
                answer.Id,
                answer.QuizQuestionId,
                answer.SelectedAnswerOptionId));
    }

    private async Task<Quiz> GetOwnedQuizAsync(
        Guid quizId,
        Guid userId)
    {
        Quiz? quiz = await _quizRepository.GetQuizById(quizId);

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

    private void EnsureStudent()
    {
        if (_currentUserService.Role != "Student")
        {
            throw new UnauthorizedAccessException(
                "Only students can submit quizzes.");
        }
    }
}