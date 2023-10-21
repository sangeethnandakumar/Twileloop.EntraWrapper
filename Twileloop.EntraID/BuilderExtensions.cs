using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;
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

            //Validate Options
            services.Configure<EntraConfig>(authOptions.EntraConfig);
            var entraConfig = authOptions.EntraConfig.Get<EntraConfig>();

            AddAuthentication(services, entraConfig, authOptions);
            AddAuthorization(services, entraConfig, authOptions);
            AddDependentServices(services);

            return services;
        }

        public static void AddDependentServices(IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, RoleValidationHandler>();
        }

        public static void AddAuthorization(IServiceCollection services, EntraConfig entraConfig, SecurityOptions authOptions)
        {
            //Authorization
            services.AddAuthorization(options =>
            {
                foreach (AuthorizationPolicy policy in entraConfig.TokenValidation.AuthorizationPolicies)
                {
                    options.AddPolicy(policy.Name, p =>
                    {
                        p.AddRequirements(new RoleValidationRequirement(policy));
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
                        OnTokenValidated = x =>
                        {
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = x =>
                        {
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
