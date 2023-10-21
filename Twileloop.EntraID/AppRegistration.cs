using System.Collections.Generic;

namespace Twileloop.EntraID
{
    public record AppRegistration
    {
        public string Name { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }


}
