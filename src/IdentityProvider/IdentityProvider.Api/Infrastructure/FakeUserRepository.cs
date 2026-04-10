using IdentityProvider.Api.Domain;

namespace IdentityProvider.Api.Infrastructure;

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<string, UserIdentity> _users = new Dictionary<string, UserIdentity>
    {
        { "john", new UserIdentity { Username = "john.doe", FirstName = "John", LastName="Smith", Email = "john.doe@example.com", Phone="555-123-000" , HashedPassword = "password123", Permissions = ["create", "print"] } },
        { "jane", new UserIdentity { Username = "jane.smith", FirstName="Jane ",  LastName="Smith", Email = "jane.smith@example.com", Phone= "555-456-000", HashedPassword = "password456", Roles = ["manager"] } }
    };
    
    public Task<UserIdentity?> GetByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);

        return Task.FromResult(user);
    }
}
