using System.Collections.Generic;

namespace Twileloop.EntraWrapper.ConfigModels
{
    public record TokenValidation
    {
        public bool Enable { get; set; } = true;
        public IEnumerable<AuthorizationPolicy> AuthorizationPolicies { get; set; }
    }
}
