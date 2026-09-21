using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LMSBackend.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                Activity.Current?.Id ?? context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException =>
                (StatusCodes.Status400BadRequest, "Validation Error"),

            NotFoundException =>
                (StatusCodes.Status404NotFound, "Not Found"),

            ConflictException =>
                (StatusCodes.Status409Conflict, "Conflict"),

            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = GetDetail(exception, statusCode),
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] =
            Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private string GetDetail(Exception exception, int statusCode)
    {
        // Show full exception information during development.
        if (_env.IsDevelopment())
        {
            return exception.ToString();
        }

        // Never expose unexpected internal errors in production.
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            return "An unexpected error occurred.";
        }

        // Custom exceptions contain messages that are safe to return.
        return exception.Message;
    }
}