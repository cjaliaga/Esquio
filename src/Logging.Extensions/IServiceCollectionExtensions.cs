using Logging.Extensions.Filters;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomApplicationInsightsTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddApplicationInsightsTelemetry(configuration)
                .AddApplicationInsightsTelemetryProcessor<LivenessTelemetryFilter>()
                .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, options) =>
                {
                    module.EnableSqlCommandTextInstrumentation = true;
                });
        }
    }
}
