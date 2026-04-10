using IdentityProvider.Api.Domain;

namespace IdentityProvider.Api.Infrastructure;

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<string, UserIdentity> _users = new Dictionary<string, UserIdentity>
    {
        { "john", new UserIdentity { Username = "john.doe", FirstName = "John", LastName="Smith", HashedPassword = "password123" } },
        { "jane", new UserIdentity { Username = "jane.smith", FirstName="Jane ",  LastName="Smith", HashedPassword = "password456" } }
    };

    public Task<UserIdentity?> GetByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);

        return Task.FromResult(user);
    }
}
