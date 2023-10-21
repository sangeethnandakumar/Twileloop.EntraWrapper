using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Twileloop.EntraID
{
    public interface IEntraAuthorizationResolver
    {
        EntraAuthorizationResult ValidatePolicyAuthorization(HttpContext context, AuthorizationPolicy policy, JwtSecurityToken token);
    }

}
