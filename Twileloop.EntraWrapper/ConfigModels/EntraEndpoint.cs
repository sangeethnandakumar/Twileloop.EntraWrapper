namespace Twileloop.EntraWrapper.ConfigModels
{
    public record EntraEndpoint
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string Policy { get; set; }
        public string Version { get; set; }
    }


}
