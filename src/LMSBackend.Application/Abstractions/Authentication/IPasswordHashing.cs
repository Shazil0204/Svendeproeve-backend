namespace LMSBackend.Application.Abstractions.Authentication;

public interface IPasswordHashing
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
