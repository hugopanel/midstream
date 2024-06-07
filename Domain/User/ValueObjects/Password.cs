using Microsoft.AspNetCore.Identity;

namespace Domain.User.ValueObjects;

public class Password
{
    public string HashedPassword { get; private set; }
    
    public Password(string hashedPassword)
    {
        HashedPassword = hashedPassword;
    }

    private Password() // EF Core
    {
        
    }

    public static Password FromPlainText(string plainTextPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var hashedPassword = passwordHasher.HashPassword(null!, plainTextPassword);
        return new Password(hashedPassword);
    }

    public static Password FromHashed(string hashedPassword)
    {
        return new Password(hashedPassword);
    }
    
    public bool Verify(string plainTextPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(null!, HashedPassword, plainTextPassword);
        return result == PasswordVerificationResult.Success;
    }

    public override string ToString()
    {
        return HashedPassword;
    }
}