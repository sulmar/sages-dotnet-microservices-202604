using IdentityProvider.Api.Domain;

namespace IdentityProvider.Api.Infrastructure;

public class FakeAuthService(IUserRepository repository) : IAuthService
{
    public async Task<ValidationResult> ValidateAsync(string username, string password)
    {
        var user = await repository.GetByUsernameAsync(username);

        ValidationResult validationResult = new ValidationResult(
            IsValid: user != null && password == user.HashedPassword, // For demo purposes, we use a hardcoded password
            Identity: user ?? new UserIdentity(),
            ErrorMessage: user == null ? "User not found" : "Invalid password"
        );

        return validationResult;
    }
}
