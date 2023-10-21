namespace Twileloop.EntraID.DemoApi.EntraID
{
    public class MyConfigResolver : IEntraConfigurationResolver
    {
        private readonly IConfiguration configuration;

        public MyConfigResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public EntraConfig Resolve()
        {
            var config = configuration.GetSection("EntraConfig").Get<EntraConfig>();
            return config;
        }
    }
}
