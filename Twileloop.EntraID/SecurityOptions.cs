namespace Twileloop.EntraID
{

    public record SecurityOptions
    {
        public bool Enable { get; set; } = true;
        public bool EnableEventLogging { get; set; } = false;
        public IEntraConfigurationResolver ConfigurationResolver { get; set; }
        public IEntraEventLogger SecurityEventLogger { get; set; }
    }


}
