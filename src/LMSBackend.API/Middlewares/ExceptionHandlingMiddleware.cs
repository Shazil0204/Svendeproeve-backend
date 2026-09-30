using System.Diagnostics;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LMSBackend.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var traceId =
                Activity.Current?.Id ??
                context.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                traceId);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await HandleExceptionAsync(
                context,
                exception,
                traceId);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string traceId)
    {
        var showDetails =
            _environment.IsDevelopment() ||
            _environment.IsStaging();

        var (statusCode, title, detail) = exception switch
        {
            // -------------------------
            // Domain / Application
            // -------------------------

            DomainValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                ex.Message),

            ValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                ex.Message),

            UnauthorizedAccessException ex => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message),

            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Not Found",
                ex.Message),

            ConflictException ex => (
                StatusCodes.Status409Conflict,
                "Conflict",
                ex.Message),

            // -------------------------
            // PostgreSQL / EF Core
            // -------------------------

            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                }
            } => (
                StatusCodes.Status409Conflict,
                "Database Conflict",
                "A record with the same unique value already exists."),

            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.ForeignKeyViolation
                }
            } => (
                StatusCodes.Status409Conflict,
                "Database Conflict",
                "The operation conflicts with related data."),

            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.NotNullViolation
                }
            } => (
                StatusCodes.Status400BadRequest,
                "Database Validation Error",
                "A required database value is missing."),

            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency Conflict",
                "The record was modified or deleted by another operation."),

            DbUpdateException ex => (
                StatusCodes.Status500InternalServerError,
                "Database Error",
                showDetails
                    ? GetDetailedMessage(ex)
                    : "A database error occurred."),

            PostgresException ex => (
                StatusCodes.Status500InternalServerError,
                "Database Error",
                showDetails
                    ? GetDetailedMessage(ex)
                    : "A database error occurred."),

            // -------------------------
            // Common .NET exceptions
            // -------------------------

            ArgumentNullException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                ex.Message),

            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                ex.Message),

            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                ex.Message),

            // -------------------------
            // Everything else
            // -------------------------

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                showDetails
                    ? GetDetailedMessage(exception)
                    : "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        // Extra debugging information ONLY outside Production.
        if (showDetails)
        {
            problem.Extensions["exceptionType"] =
                exception.GetType().FullName;

            if (exception.InnerException is not null)
            {
                problem.Extensions["innerExceptionType"] =
                    exception.InnerException.GetType().FullName;

                problem.Extensions["innerException"] =
                    exception.InnerException.Message;
            }
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string GetDetailedMessage(Exception exception)
    {
        var messages = new List<string>();

        Exception? current = exception;

        while (current is not null)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                messages.Add(current.Message);
            }

            current = current.InnerException;
        }

        return string.Join(" --> ", messages);
    }
}