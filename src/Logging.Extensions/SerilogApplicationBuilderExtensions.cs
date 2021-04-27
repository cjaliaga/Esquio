using Logging.Extensions.UserLogging;

namespace Microsoft.AspNetCore.Builder
{
    public static class SerilogApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSerilogUserLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserLoggingMiddleware>();
        }
    }
}
