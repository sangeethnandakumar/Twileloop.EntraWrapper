using System.Collections.Generic;

namespace Twileloop.EntraWrapper
{
    public record ClaimRequirements
    {
        public string Type { get; set; }
        public IEnumerable<string> Values { get; init; }
    }
}
