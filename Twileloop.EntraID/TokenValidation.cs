using System.Collections.Generic;

namespace Twileloop.EntraID
{
    public record TokenValidation

    {
        public IEnumerable<AuthorizationPolicy> AuthorizationPolicies { get; set; }
    }


}
