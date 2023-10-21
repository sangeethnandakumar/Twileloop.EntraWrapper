using System.Collections.Generic;

namespace Twileloop.EntraID
{
    public record ClaimRequirements
    {
        public string Type { get; set; }
        public IEnumerable<string> Values { get; init; }
    }
}
