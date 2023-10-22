using Microsoft.AspNetCore.Authorization;

namespace Twileloop.EntraWrapper
{
    public class RoleValidationRequirement : IAuthorizationRequirement
    {
        public AuthorizationPolicy Policy { get; }

        public RoleValidationRequirement(AuthorizationPolicy policy)
        {
            Policy = policy;
        }
    }
}
