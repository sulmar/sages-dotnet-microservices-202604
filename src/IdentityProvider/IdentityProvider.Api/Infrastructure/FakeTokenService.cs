using IdentityProvider.Api.Domain;

namespace IdentityProvider.Api.Infrastructure;

public class FakeTokenService : ITokenService
{
    public string GenerateToken(UserIdentity identity)
    {
        return "abc";
    }
}