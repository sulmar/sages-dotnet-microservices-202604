namespace IdentityProvider.Api.Domain;

public interface IAuthService
{
    Task<ValidationResult> ValidateAsync(string username, string password);
}

public record ValidationResult(bool IsValid, UserIdentity Identity, string? ErrorMessage = null);


public class UserIdentity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; } = "password123";
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string[] Roles { get; set; }
    public string[] Permissions { get; set; }
}


public interface ITokenService
{
    string GenerateToken(UserIdentity identity);
}

public interface IUserRepository
{
    Task<UserIdentity?> GetByUsernameAsync(string username);
}