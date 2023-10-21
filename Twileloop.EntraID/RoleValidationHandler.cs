using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Twileloop.EntraID
{
    public class RoleValidationHandler : AuthorizationHandler<RoleValidationRequirement>
    {
        public RoleValidationHandler()
        {
            
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, RoleValidationRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
