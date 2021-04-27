using System;
using Microsoft.Extensions.Configuration;
using WebHost.Extensions.ConfigurationProviders.AzureAppConfiguration;

namespace Microsoft.AspNetCore.Hosting
{
    public static class AzureAppConfigurationWebHostExtensions
    {
        /// <summary>
        /// Adds support for Azure App Configuration in an automated way.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param> 
        /// <param name="setup">The action to configure <see cref="AzureAppConfigurationExtendedOptions"/>. If not defined it will look for the section <c>ConfigurationProviders:AzureAppConfiguration</c> in <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseAzureAppConfiguration(this IWebHostBuilder builder, Action<AzureAppConfigurationExtendedOptions> setup = null)
        {
            AzureAppConfigurationExtendedOptions azureAppConfig = null;
            if (setup != null)
            {
                azureAppConfig = new AzureAppConfigurationExtendedOptions();
                setup.Invoke(azureAppConfig);
            }

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var settings = config.Build();
                azureAppConfig ??= settings.GetAzureAppConfigurationExtendedOptions();

                if (azureAppConfig.Enabled)
                {
                    config.AddAzureAppConfiguration(azureAppConfig);
                }

            });

            return builder;
        }

    }
}
