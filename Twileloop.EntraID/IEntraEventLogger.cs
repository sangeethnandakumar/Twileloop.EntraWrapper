namespace Twileloop.EntraID
{
    public interface IEntraEventLogger
    {
        void OnInfo(string message, EntraConfig entraConfig, SecurityOptions securityOptions);
        void OnSuccess(string message, EntraConfig entraConfig, SecurityOptions securityOptions);
        void OnFailure(string message, EntraConfig entraConfig, SecurityOptions securityOptions);
    }


}
