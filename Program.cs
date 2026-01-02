using EmailSenderApp.Extensions;

namespace EmailSenderApp;

public static partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ConfigureLogging();
        builder.Services.ConfigureEnvSettings();
        builder.Services.ConfigureServices();
        builder.Services.ConfigureCors();
        builder.Services.AddControllers();
        builder.Services.ConfigureRateLimit();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseRateLimiter();
        app.ConfigureMiddleware();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok()).WithTags("Health");
        app.Run();
    }
}
