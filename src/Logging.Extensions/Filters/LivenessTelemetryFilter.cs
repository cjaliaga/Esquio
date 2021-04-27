using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Logging.Extensions.Filters
{
    public class LivenessTelemetryFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        static readonly string _discarded = "/liveness";

        public LivenessTelemetryFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry request && request.Url.AbsolutePath == _discarded)
            {
                return;
            }

            _next.Process(item);
        }
    }
}

