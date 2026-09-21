using LMSBackend.API.Middlewares;
using LMSBackend.API.Services;
using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Services.Auditing;
using LMSBackend.Infrastructure.Auditing;
using LMSBackend.Infrastructure.Data;
using LMSBackend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.TraversePath().Load();

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

string baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL")
    ?? throw new InvalidOperationException("APP_BASE_URL is not configured.");

builder.Services.AddHttpContextAccessor();

// Dependencies Injection
builder.Services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var auditInterceptor =
        serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>();

    options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(auditInterceptor);
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

WebApplication? app = builder.Build();

// Catch exceptions from everything that runs after this middleware. 
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
