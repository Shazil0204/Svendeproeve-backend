using System.Text.RegularExpressions;
using LMSBackend.Application.Abstractions.Authentication;

namespace LMSBackend.Infrastructure.Authentication;

public class PasswordHashing : IPasswordHashing
{
    private static readonly Regex PasswordPolicy = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        if (!PasswordPolicy.IsMatch(password))
        {
            throw new ArgumentException("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
