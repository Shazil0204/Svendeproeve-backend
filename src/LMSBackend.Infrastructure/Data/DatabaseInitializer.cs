using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Domain.Entities.Users;
using LMSBackend.Domain.Enums.Users;
using LMSBackend.Domain.ValueObjects.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LMSBackend.Infrastructure.Data;

public sealed class DatabaseInitializer(
    AppDbContext dbContext,
    IPasswordHashing passwordHashing,
    IConfiguration configuration,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Applying pending database migrations.");

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Database migrations are up to date.");

        await SeedAdministratorAsync(cancellationToken);
    }

    private async Task SeedAdministratorAsync(CancellationToken cancellationToken)
    {
        var administratorExists = await dbContext.Users
            .AnyAsync(
                user => user.Role == UserRole.Administrator,
                cancellationToken);

        if (administratorExists)
        {
            logger.LogInformation(
                "Administrator already exists. Skipping administrator seed.");

            return;
        }

        var name = configuration["ADMIN_NAME"];
        var email = configuration["ADMIN_EMAIL"];
        var password = configuration["ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Administrator seed configuration is missing.");
        }

        var passwordHash = passwordHashing.HashPassword(password);

        var administrator = new User(
            name,
            new Email(email),
            passwordHash,
            UserRole.Administrator);

        dbContext.Users.Add(administrator);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Initial administrator created.");
    }
}