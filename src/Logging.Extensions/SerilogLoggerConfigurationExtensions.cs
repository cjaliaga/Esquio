using System;
using Logging.Extensions;
using Logging.Extensions.Converters;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;

namespace Serilog
{
    public static class SerilogLoggerConfigurationExtensions
    {
        public static LoggerConfiguration ConfigureSinks(this LoggerConfiguration logger, HostBuilderContext hostContext, IServiceProvider services)
        {
            logger.ReadFrom.Configuration(hostContext.Configuration)
                  .Enrich.FromLogContext()
                  .Enrich.WithProperty(nameof(hostContext.HostingEnvironment.ApplicationName), hostContext.HostingEnvironment.ApplicationName)
                  .Filter.ByExcluding("EndsWith(RequestPath, '/liveness')")
                  .Filter.ByExcluding("EndsWith(RequestPath, '/healthz')")
                  .WriteTo.Console();

            var telemetryClient = services.GetService<TelemetryClient>();
            if (telemetryClient != null)
            {
                logger.WriteTo.ApplicationInsights(telemetryClient, new ApplicationInsightsCustomConverter());
            }

            var elasticsearch = hostContext.Configuration.GetConnectionString(Constants.Elasticsearch);

            if (!string.IsNullOrEmpty(elasticsearch))
            {
                logger.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearch))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                });
            }

            var seq = hostContext.Configuration.GetConnectionString(Constants.Seq);

            if (!string.IsNullOrEmpty(seq))
            {
                logger.WriteTo.Seq(seq);
            }

            return logger;
        }
       
    }
}
