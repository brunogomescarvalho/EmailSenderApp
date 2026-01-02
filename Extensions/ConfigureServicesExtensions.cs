using EmailSenderApp.Services;

namespace EmailSenderApp.Extensions;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IEmailSender, EmailSender>();
    }
}
