using System.Collections.Generic;

namespace Twileloop.EntraWrapper.ConfigModels
{
    public record AuthorizationPolicy
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public IEnumerable<ClaimRequirements> Claims { get; set; }
    }
}
