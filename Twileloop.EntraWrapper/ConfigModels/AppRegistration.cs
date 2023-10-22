using System.Collections.Generic;

namespace Twileloop.EntraWrapper.ConfigModels
{
    public record AppRegistration
    {
        public string Name { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }


}
