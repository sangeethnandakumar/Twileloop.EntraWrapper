namespace Twileloop.EntraID.DemoApi.EntraID
{
    public class MyLogger : IEntraEventLogger
    {
        private readonly ILogger<MyLogger> logger;

        public MyLogger(ILogger<MyLogger> logger)
        {
            this.logger = logger;
        }

        public void OnFailure(string message, EntraConfig entraConfig, SecurityOptions securityOptions)
        {
            logger.LogError(message);
        }

        public void OnInfo(string message, EntraConfig entraConfig, SecurityOptions securityOptions)
        {
            logger.LogWarning(message);
        }

        public void OnSuccess(string message, EntraConfig entraConfig, SecurityOptions securityOptions)
        {
            logger.LogInformation(message);
        }
    }
}
