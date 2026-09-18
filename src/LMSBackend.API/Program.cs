using LMSBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.TraversePath().Load();

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

string baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL")
    ?? throw new InvalidOperationException("APP_BASE_URL is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

WebApplication? app = builder.Build();

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
