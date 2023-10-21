namespace Twileloop.EntraID
{
    public record SecurityOptions
    {
        public bool EnableEventLogging { get; set; } = false;
        public string GlobalAuthenticationFailureResponse { get; set; } = string.Empty;
        public string GlobalAuthorizationFailureResponse { get; set; } = string.Empty;
        public IEntraConfigurationResolver ConfigurationResolver { get; set; }
        public IEntraAuthorizationResolver AuthorizationResolver { get; set; }
        public IEntraEventLogger SecurityEventLogger { get; set; }
    }

}
