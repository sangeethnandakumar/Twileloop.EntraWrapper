using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Twileloop.EntraWrapper.ConfigModels;
using Twileloop.EntraWrapper.Extensions;
using Twileloop.EntraWrapper.Models;

namespace Twileloop.EntraWrapper.AuthorizationDrivers
{

    public class AuthorizationPolicyHandler : AuthorizationHandler<AuthorizationPolicyRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptions<SecurityOptions> securityOptions;
        private readonly IOptions<EntraConfig> entraConfig;
        private readonly SecurityLogger securityLogger;
        private readonly JwtSecurityTokenHandler tokenHandler;

        public AuthorizationPolicyHandler(IOptions<SecurityOptions> securityOptions, IOptions<EntraConfig> entraConfig, SecurityLogger securityLogger, IHttpContextAccessor httpContextAccessor)
        {
            this.securityOptions = securityOptions;
            this.entraConfig = entraConfig;
            this.securityLogger = securityLogger;
            this.httpContextAccessor = httpContextAccessor;
            tokenHandler = new JwtSecurityTokenHandler();
        }


        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AuthorizationPolicyRequirement requirement)
        {
            if (httpContextAccessor.HttpContext.Response.StatusCode == 401)
            {
                return;
            }

            if (!entraConfig.Value.TokenValidation.Enable)
            {
                securityLogger.LogInfo($"Authorization is disabled globally. Bypassing authorization resolver...");
                context.Succeed(requirement);
                return;
            }

            if (!requirement.Policy.Enable)
            {
                securityLogger.LogInfo($"Security policy '{requirement.Policy.Name}' is disabled. Bypassing policy...");
                context.Succeed(requirement);
                return;
            }

            //Read token
            securityLogger.LogInfo("Decoding JWT claims...");
            var bearerToken = httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if(bearerToken is null)
            {
                securityLogger.LogFailure("No JWT security token found in request. Ensure if bearer token is propery sent in 'Authorization' header");
                context.Fail();
                httpContextAccessor.HttpContext.Response.OnStarting(async () =>
                {
                    httpContextAccessor.HttpContext.Response.StatusCode = 401;
                    httpContextAccessor.HttpContext.Response.ContentType = "text/plain";
                    await httpContextAccessor.HttpContext.Response.WriteAsync(securityOptions.Value.GlobalAuthenticationFailureResponse);
                });
                securityLogger.LogFailure("Emiting 401 UNAUTHORIZED");
                return;
            }

            var token = bearerToken.Replace("Bearer ", string.Empty);
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            // Resolve claim authorization
            securityLogger.LogInfo("Attempting delegated authorization...");
            var authorizationResult = securityOptions.Value.AuthorizationResolver.ValidatePolicyAuthorization(httpContextAccessor.HttpContext, requirement.Policy, jwtSecurityToken);
            if (authorizationResult.IsAuthorised)
            {
                securityLogger.LogSuccess("Reported successfull authorization attempt");
                securityLogger.LogSuccess("Provisioning endpoint access...");
                context.Succeed(requirement);
                return;
            }
            securityLogger.LogFailure("Reported failure authorization attempt");
            securityLogger.LogFailure($"Emiting '{authorizationResult.OverrideAuthorizationFailureResponse ?? securityOptions.Value.GlobalAuthorizationFailureResponse}'");
            securityLogger.LogFailure("Emiting 403 FORBIDDEN");
            context.Fail();
            httpContextAccessor.HttpContext.Response.StatusCode = 403;
            await httpContextAccessor.HttpContext.Response.WriteAsync(authorizationResult.OverrideAuthorizationFailureResponse ?? securityOptions.Value.GlobalAuthorizationFailureResponse);
        }
    }
}
