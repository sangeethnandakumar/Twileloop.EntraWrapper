using System.Collections.Generic;

namespace Twileloop.EntraID
{
    public record AuthorizationPolicy
    {
        public bool Enable { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> ClientAccessTokenRoles { get; set; }
        public IEnumerable<string> UserAccessTokenRoles { get; set; }
    }


}
