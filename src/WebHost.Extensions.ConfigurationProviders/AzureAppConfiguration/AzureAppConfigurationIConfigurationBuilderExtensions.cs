using System;
using Azure.Identity;
using WebHost.Extensions.ConfigurationProviders.AzureAppConfiguration;

namespace Microsoft.Extensions.Configuration
{
    public static class AzureAppConfigurationIConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureAppConfiguration(this IConfigurationBuilder builder, AzureAppConfigurationExtendedOptions azureAppConfig)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (azureAppConfig is null)
            {
                throw new ArgumentNullException(nameof(azureAppConfig));
            }

            builder.AddAzureAppConfiguration(options =>
            {
                if (azureAppConfig.UseIdentity)
                {
                    options.Connect(new Uri(azureAppConfig.ConnectionString), new ManagedIdentityCredential(azureAppConfig.ClientId));
                }
                else
                {
                    options.Connect(azureAppConfig.ConnectionString);
                }

                azureAppConfig.Labels.ForEach(action => options.Select("*", action));
            });

            return builder;
        }
    }
}
