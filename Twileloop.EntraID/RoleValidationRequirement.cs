using Microsoft.AspNetCore.Authorization;

namespace Twileloop.EntraID
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
