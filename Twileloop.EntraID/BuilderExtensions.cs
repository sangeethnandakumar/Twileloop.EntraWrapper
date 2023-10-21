using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace Twileloop.EntraID
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddEntraID(this IServiceCollection services, Action<SecurityOptions> authOptionDelegate)
        {
            //Bind options
            var authOptions = new SecurityOptions();
            authOptionDelegate(authOptions);
            var entraConfig = authOptions.ConfigurationResolver.Resolve();

            AddAuthentication(services, entraConfig, authOptions);
            AddAuthorization(services, entraConfig, authOptions);

            services.AddTransient<IAuthorizationHandler, RoleValidationHandler>();
            return services;
        }

        private static void LogInfo(string message, EntraConfig entraConfig, SecurityOptions authOptions)
        {
            if (authOptions.EnableEventLogging)
            {
                authOptions.SecurityEventLogger.OnInfo(message, entraConfig, authOptions);
            }
        }

        public static void AddAuthorization(IServiceCollection services, EntraConfig entraConfig, SecurityOptions authOptions)
        {
            //Authorization
            services.AddAuthorization(options =>
            {
                var activePolicies = entraConfig.TokenValidation.AuthorizationPolicies.Where(p => p.Enable).ToList();
                LogInfo($"Found {activePolicies.Count} active authorization policies", entraConfig, authOptions);
                foreach (AuthorizationPolicy policy in activePolicies)
                {
                    LogInfo($"Registering policy '{policy.Name}'", entraConfig, authOptions);
                    options.AddPolicy(policy.Name, p =>
                    {
                        p.AddRequirements(new RoleValidationRequirement(policy));
                        p.RequireAuthenticatedUser();
                    });
                }
            });
        }

        public static void AddAuthentication(IServiceCollection services, EntraConfig entraConfig, SecurityOptions authOptions)
        {
            //Authentication
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(jwtBearerOptions =>
                {
                    jwtBearerOptions.Audience = entraConfig.ClientId;
                    jwtBearerOptions.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (authOptions.EnableEventLogging)
                            {
                                if (context.Exception is SecurityTokenExpiredException)
                                {
                                    authOptions.SecurityEventLogger.OnSuccess("JWT token is expired", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                                else if (context.Exception is SecurityTokenInvalidLifetimeException)
                                {
                                    authOptions.SecurityEventLogger.OnSuccess("JWT token lifetime is invalid", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                                else if (context.Exception is SecurityTokenNotYetValidException)
                                {
                                    authOptions.SecurityEventLogger.OnSuccess("JWT token is not yet valid. Check the 'nbf' claim", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                                else if (context.Exception is SecurityTokenMalformedException)
                                {
                                    authOptions.SecurityEventLogger.OnFailure("JWT token is tampered or malformed", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                                else if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                                {
                                    authOptions.SecurityEventLogger.OnFailure("JWT token signature cannot be verified from JWK endpoint", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                                else
                                {
                                    authOptions.SecurityEventLogger.OnFailure($"Token validation failed", entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnSuccess(context.Exception.Message, entraConfig, authOptions);
                                    authOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED", entraConfig, authOptions);
                                }
                            }

                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                },
                microsoftIdentityOptions =>
                {
                    microsoftIdentityOptions.Instance = entraConfig.EntraEndpoint.Instance;
                    microsoftIdentityOptions.Domain = entraConfig.EntraEndpoint.Domain;
                    microsoftIdentityOptions.TenantId = entraConfig.EntraEndpoint.TenantId;
                    microsoftIdentityOptions.Authority = entraConfig.Authority;
                    microsoftIdentityOptions.ClientId = entraConfig.ClientId;
                    microsoftIdentityOptions.ClientSecret = entraConfig.TokenGeneration.ClientSecret;
                });
        }
    }
}
