using System.Threading.RateLimiting;

namespace EmailSenderApp.Extensions;

public static class ConfigureRateLimitExtensions
{
    public static void ConfigureRateLimit(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(10),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };
        });
    }
}
