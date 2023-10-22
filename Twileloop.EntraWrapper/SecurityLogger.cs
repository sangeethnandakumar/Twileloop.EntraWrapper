using Microsoft.Extensions.Options;

namespace Twileloop.EntraWrapper
{
    public class SecurityLogger
    {
        private readonly IOptions<SecurityOptions> securityOptions;

        public SecurityLogger(IOptions<SecurityOptions> securityOptions)
        {
            this.securityOptions = securityOptions;
        }

        public void LogInfo(string message)
        {
            if (securityOptions.Value.EnableEventLogging)
            {
                securityOptions.Value.SecurityEventLogger.OnInfo(message);
            }
        }

        public void LogSuccess(string message)
        {
            if (securityOptions.Value.EnableEventLogging)
            {
                securityOptions.Value.SecurityEventLogger.OnSuccess(message);
            }
        }

        public void LogFailure(string message)
        {
            if (securityOptions.Value.EnableEventLogging)
            {
                securityOptions.Value.SecurityEventLogger.OnFailure(message);
            }
        }
    }
}
