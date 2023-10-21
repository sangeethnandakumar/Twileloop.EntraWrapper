namespace Twileloop.EntraID
{
    public record EntraEndpoint
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string Policy { get; set; }
        public string Version { get; set; }
    }

    public record EntraConfig
    {
        public string AppName { get; set; }
        public string ClientId { get; set; }

        public EntraEndpoint EntraEndpoint { get; set; }

        internal string Authority
        {
            get { return $"{EntraEndpoint.Instance}/tfp/{EntraEndpoint.TenantId}/{EntraEndpoint.Policy}/{EntraEndpoint.Version}"; }
        }

        public TokenGeneration TokenGeneration { get; set; }
        public TokenValidation TokenValidation { get; set; }
    }


}
