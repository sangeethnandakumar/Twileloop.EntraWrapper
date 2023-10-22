using Microsoft.AspNetCore.Authorization;
using AuthorizationPolicy = Twileloop.EntraWrapper.ConfigModels.AuthorizationPolicy;

namespace Twileloop.EntraWrapper.AuthorizationDrivers
{
    public class AuthorizationPolicyRequirement : IAuthorizationRequirement
    {
        public AuthorizationPolicy Policy { get; }

        public AuthorizationPolicyRequirement(AuthorizationPolicy policy)
        {
            Policy = policy;
        }
    }
}
