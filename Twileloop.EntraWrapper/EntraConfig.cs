namespace Twileloop.EntraWrapper
{
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
