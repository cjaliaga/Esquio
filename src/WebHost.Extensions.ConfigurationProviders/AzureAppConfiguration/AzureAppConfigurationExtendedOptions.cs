using System.Collections.Generic;

namespace WebHost.Extensions.ConfigurationProviders.AzureAppConfiguration
{
    public class AzureAppConfigurationExtendedOptions
    {
        public bool Enabled { get; set; } = false;
        public bool UseIdentity { get; set; } = false;
        public string ClientId { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        public string ConnectionString { get; set; }
    }
}
