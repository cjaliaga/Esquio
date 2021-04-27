using System;
using WebHost.Extensions.ConfigurationProviders.AzureAppConfiguration;

namespace Microsoft.Extensions.Configuration
{
    public static class AzureAppConfigurationIConfigurationRootExtensions
    {
        private const string ConnectionStringName = "AzureAppConfiguration";
        private const string ConfigurationSection = "ConfigurationProviders:AzureAppConfiguration";

        public static AzureAppConfigurationExtendedOptions GetAzureAppConfigurationExtendedOptions(this IConfigurationRoot root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var options = root.GetSection(ConfigurationSection).Get<AzureAppConfigurationExtendedOptions>();

            if(options.Enabled && string.IsNullOrEmpty(options.ConnectionString))
            {
                options.ConnectionString = root.GetConnectionString(ConnectionStringName);
            }

            return options;
        }

    }
}
