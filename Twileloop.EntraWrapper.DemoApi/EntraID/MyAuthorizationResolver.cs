using Microsoft.AspNetCore.Http.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace Twileloop.EntraWrapper.DemoApi.EntraID
{
    public class MyAuthorizationResolver : IEntraAuthorizationResolver
    {
        public EntraAuthorizationResult ValidatePolicyAuthorization(HttpContext context, AuthorizationPolicy policy, JwtSecurityToken token)
        {

            //Get all scopes from token
            var tokenScopes = token.Claims.Where(x => x.Type == "scp").Select(x => x.Value);
            //Get all scopes required
            var policyScopes = policy.Claims.FirstOrDefault(x => x.Type == "scp")?.Values;
            //Simply check if all required scopes are met
            var isScopesMet = policyScopes.Intersect(tokenScopes).Count() == policyScopes.Count();

            return new EntraAuthorizationResult(isScopesMet, $"Sorry you don't have the following permissions: {string.Join(", ", policyScopes.Except(tokenScopes))} for endpoint: {context.Request.GetDisplayUrl()}");
        }
    }
}
