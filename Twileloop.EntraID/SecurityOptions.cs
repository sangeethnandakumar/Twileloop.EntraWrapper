using Microsoft.Extensions.Configuration;
using System;

namespace Twileloop.EntraID
{
    public record SecurityOptions
    {
        public bool Enable { get; set; } = true;
        public bool EnableEventLogging { get; set; } = false;
        public IConfigurationSection EntraConfig { get; set; }
        public Action<string> OnSecurityEventLog { get; set; }
    }


}
