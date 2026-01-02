namespace EmailSenderApp.Extensions;

public static class ConfigureLoggingExtensions
{
    public static void ConfigureLogging(this ILoggingBuilder logging)
    {
        logging.ClearProviders()
               .AddConsole()
               .SetMinimumLevel(LogLevel.Information);
    }
}
