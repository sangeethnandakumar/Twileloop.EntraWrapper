using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Twileloop.EntraID
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddEntraID(this IServiceCollection services, Action<SecurityOptions> authOptionDelegate)
        {
            //Bind options
            var securityOptions = new SecurityOptions();
            authOptionDelegate(securityOptions);
            var entraConfig = securityOptions.ConfigurationResolver.Resolve();

            AddAuthentication(services, entraConfig, securityOptions);
            AddAuthorization(services, entraConfig, securityOptions);

            services.AddSingleton(Options.Create(securityOptions));
            services.AddSingleton(Options.Create(entraConfig));
            services.AddSingleton<SecurityLogger>();
            services.AddTransient<IAuthorizationHandler, RoleValidationHandler>();
            return services;
        }

        private static void LogInfo(string message, SecurityOptions securityOptions)
        {
            if (securityOptions.EnableEventLogging)
            {
                securityOptions.SecurityEventLogger.OnInfo(message);
            }
        }

        public static void AddAuthorization(IServiceCollection services, EntraConfig entraConfig, SecurityOptions securityOptions)
        {
            //Authorization
            services.AddAuthorization(options =>
            {
                var activePolicies = entraConfig.TokenValidation.AuthorizationPolicies.Where(p => p.Enable).ToList();
                LogInfo($"Found {activePolicies.Count} active authorization policies", securityOptions);
                foreach (AuthorizationPolicy policy in entraConfig.TokenValidation.AuthorizationPolicies)
                {
                    options.AddPolicy(policy.Name, p =>
                    {
                        if (policy.Enable)
                        {
                            LogInfo($"Registering policy '{policy.Name}'", securityOptions);
                        }
                        p.AddRequirements(new RoleValidationRequirement(policy));
                        if (entraConfig.TokenValidation.Enable)
                        {
                            p.RequireAuthenticatedUser();
                        }
                    });
                }
            });
        }

        public static void AddAuthentication(IServiceCollection services, EntraConfig entraConfig, SecurityOptions securityOptions)
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
                            if (entraConfig.TokenValidation.Enable && securityOptions.EnableEventLogging)
                            {
                                securityOptions.SecurityEventLogger.OnSuccess("JWT token successfully validated");
                            }
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            if (entraConfig.TokenValidation.Enable && securityOptions.EnableEventLogging)
                            {
                                securityOptions.SecurityEventLogger.OnInfo("Security check in progress...");
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = async context =>
                        {
                            if (entraConfig.TokenValidation.Enable && securityOptions.EnableEventLogging)
                            {
                                if (context.Exception is SecurityTokenExpiredException)
                                {
                                    securityOptions.SecurityEventLogger.OnFailure("JWT token is expired");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                                else if (context.Exception is SecurityTokenInvalidLifetimeException)
                                {
                                    securityOptions.SecurityEventLogger.OnFailure("JWT token lifetime is invalid");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                                else if (context.Exception is SecurityTokenNotYetValidException)
                                {
                                    securityOptions.SecurityEventLogger.OnFailure("JWT token is not yet valid. Check the 'nbf' claim");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                                else if (context.Exception is SecurityTokenMalformedException)
                                {
                                    securityOptions.SecurityEventLogger.OnFailure("JWT token is tampered or malformed");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                                else if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                                {
                                    securityOptions.SecurityEventLogger.OnFailure("JWT token signature cannot be verified from JWK endpoint");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                                else
                                {
                                    securityOptions.SecurityEventLogger.OnFailure($"Token validation failed");
                                    securityOptions.SecurityEventLogger.OnFailure(context.Exception.Message);
                                    securityOptions.SecurityEventLogger.OnFailure("Emiting 401 UNAUTHORIZED");
                                }
                            }

                            if (entraConfig.TokenValidation.Enable)
                            {
                                context.Response.Clear();
                                context.Response.StatusCode = 401;
                                context.Response.OnStarting(async () =>
                                {
                                    context.Response.ContentType = "text/plain";
                                    await context.Response.WriteAsync(securityOptions.GlobalAuthenticationFailureResponse);
                                });
                            }
                            else
                            {
                                context.Response.Clear();
                                context.Response.StatusCode = 200;
                                LogInfo($"Authentication is disabled globally. Bypassing token validation...", securityOptions);
                            }
                            return;
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
