using IdentityModel;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Logging.Extensions.UserLogging
{
    public class UserLoggingEnricher : ILogEventEnricher
    {
        private readonly HttpContext _httpContext;
        LogEventProperty _cachedProperty;

        public UserLoggingEnricher(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            _cachedProperty ??= propertyFactory.CreateProperty(Constants.UserId, _httpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value);
            logEvent.AddPropertyIfAbsent(_cachedProperty);
        }
    }
}
