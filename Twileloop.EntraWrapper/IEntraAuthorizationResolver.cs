using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Twileloop.EntraWrapper
{
    public interface IEntraAuthorizationResolver
    {
        EntraAuthorizationResult ValidatePolicyAuthorization(HttpContext context, AuthorizationPolicy policy, JwtSecurityToken token);
    }

}
