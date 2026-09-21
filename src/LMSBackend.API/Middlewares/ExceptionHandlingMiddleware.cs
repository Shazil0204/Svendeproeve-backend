using System.Diagnostics;
using LMSBackend.Application.Exceptions;
using LMSBackend.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                traceId);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await HandleExceptionAsync(context, exception, traceId);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string traceId)
    {
        var (statusCode, title, detail) = exception switch
        {
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

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        problem.Extensions["traceId"] = traceId;

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problem);
    }
}