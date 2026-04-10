using IdentityProvider.Api.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace IdentityProvider.Api.Infrastructure;

// dotnet add package Microsoft.IdentityModel.JsonWebTokens

public class FakeTokenService : ITokenService
{
    public string GenerateToken(UserIdentity identity)
    {
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Name, identity.Username },            
            { JwtRegisteredClaimNames.Email, identity.Email },
            { JwtRegisteredClaimNames.GivenName, identity.FirstName },
            { JwtRegisteredClaimNames.FamilyName, identity.LastName },
            { JwtRegisteredClaimNames.PhoneNumber, identity.Phone  },
            { ClaimTypes.Role , identity.Roles }
        };

        var secretKey = "ThisIsASecretKeyForDemoPurposesOnly";

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return token;
    }
}