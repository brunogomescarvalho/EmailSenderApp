namespace EmailSenderApp.Extensions;

public static class ConfigureMiddlewareExtensions
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == 429)
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var path = context.Request.Path;

                app.Logger.LogWarning("Requisição bloqueada por rate limit: {Path} de {IP}", path, ip);
            }
        });

    }
}
