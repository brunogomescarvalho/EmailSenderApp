using DotNetEnv;
using EmailSenderApp.Options;

namespace EmailSenderApp.Extensions;

public static class ConfigureEnvSettingsExtensions
{
    public static void ConfigureEnvSettings(this IServiceCollection services)
    {
        Env.Load();
        services.AddSingleton(new ConfigurationSettings());
    }
}
