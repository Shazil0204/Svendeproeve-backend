using System.Text;
using LMSBackend.Application.Abstractions.Submissions;
using LMSBackend.Application.Services.Submissions;
using LMSBackend.Infrastructure.Storage;
using LMSBackend.API.Middlewares;
using LMSBackend.API.Services;
using LMSBackend.Application.Abstractions.AuditLog;
using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.Abstractions.EducationalGoals;
using LMSBackend.Application.Abstractions.Groups;
using LMSBackend.Application.Abstractions.Persistence;
using LMSBackend.Application.Abstractions.Quizzes;
using LMSBackend.Application.Abstractions.Repositories;
using LMSBackend.Application.Abstractions.Subjects;
using LMSBackend.Application.Abstractions.Tasks;
using LMSBackend.Application.Abstractions.Users;
using LMSBackend.Application.Services.Auditing;
using LMSBackend.Application.Services.Authentication;
using LMSBackend.Application.Services.EducationalGoals;
using LMSBackend.Application.Services.Groups;
using LMSBackend.Application.Services.Quizzes;
using LMSBackend.Application.Services.Subjects;
using LMSBackend.Application.Services.Tasks;
using LMSBackend.Application.Services.Users;
using LMSBackend.Infrastructure.Auditing;
using LMSBackend.Infrastructure.Authentication;
using LMSBackend.Infrastructure.Data;
using LMSBackend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHashing, PasswordHashing>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IEduGoalService, EduGoalService>();
builder.Services.AddScoped<IEduGoalRepository, EduGoalRepository>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddSingleton<ISubmissionFileStore>(new LocalSubmissionFileStore(
    builder.Configuration["SubmissionStorage:RootPath"]
        ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "submissions")));

string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException(
        "JWT_SECRET is not configured.");

string jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? throw new InvalidOperationException(
        "JWT_ISSUER is not configured.");

string jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? throw new InvalidOperationException(
        "JWT_AUDIENCE is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token =
                    context.Request.Cookies["access_token"];

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append(
                        "X-Auth-Error",
                        "ACCESS_TOKEN_EXPIRED");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

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
