using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Twileloop.EntraWrapper.ConfigModels;
using Twileloop.EntraWrapper.Models;

namespace Twileloop.EntraWrapper.Abstractions
{
    public interface IEntraAuthorizationResolver
    {
        EntraAuthorizationResult ValidatePolicyAuthorization(HttpContext context, AuthorizationPolicy policy, JwtSecurityToken token);
    }

}
