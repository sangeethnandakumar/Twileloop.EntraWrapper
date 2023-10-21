using System.Collections.Generic;

namespace Twileloop.EntraID
{
    public record TokenGeneration
    {
        public string ClientSecret { get; set; }
        public IEnumerable<AppRegistration> AppRegistrations { get; set; }
    }


}
