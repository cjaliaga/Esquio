using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Logging.Extensions.UserLogging
{
    public class UserLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public UserLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            using (LogContext.Push(new UserLoggingEnricher(httpContext)))
            {
                await _next(httpContext);
            }
        }
    }
}
