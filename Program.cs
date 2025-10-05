using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderApp;

public static class Program
{
    private static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services);
        ConfigureLogging(builder.Logging);
        ConfigureCors(builder.Services);

        var app = builder.Build();
        ConfigureMiddleware(app);
        ConfigureEndpoints(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddRateLimiting();
    }

    private static void ConfigureLogging(ILoggingBuilder logging)
    {
        logging.ClearProviders()
               .AddConsole()
               .SetMinimumLevel(LogLevel.Information);
    }

    private static void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseRateLimiter();

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

    private static void ConfigureEndpoints(WebApplication app)
    {
        app.MapGet("/", () => Results.Ok("Service is alive")).DisableRateLimiting();

        app.MapPost("/contact", async (
            [FromServices] IEmailSender emailSender,
            [FromServices] ILogger<EmailRequest> logger,
            [FromBody] EmailRequest request) =>
        {
            if (!request.IsValid(out var validationResults))
                return Results.BadRequest(validationResults);

            try
            {
                await emailSender.SendEmailAsync(request);
                return Results.Ok(new { success = true, message = "Mensagem enviada com sucesso." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar e-mail");
                return Results.Problem("Ocorreu um erro ao processar sua solicitação.", statusCode: 500);
            }
        });
    }
}

#region Email Sender

public interface IEmailSender
{
    Task SendEmailAsync(EmailRequest emailRequest);
}

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        var address = Env.GetString("ADDRESS");
        var credential = Env.GetString("CREDENTIAL");

        if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(credential))
            throw new InvalidOperationException("Credenciais de e-mail ausentes.");

        using var client = new SmtpClient("smtp.gmail.com")
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(address, credential),
            Port = 587,
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(address, emailRequest.Name, System.Text.Encoding.UTF8),
            Subject = emailRequest.Subject,
            Body = emailRequest.Message,
            IsBodyHtml = true
        };

        message.To.Add(emailRequest.To);

        if (!string.IsNullOrWhiteSpace(emailRequest.From))
            message.ReplyToList.Add(new MailAddress(emailRequest.From));

        await client.SendMailAsync(message);
    }
}

#endregion

#region Models

public record EmailRequest
{
    [Required, EmailAddress]
    public string To { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string From { get; init; } = string.Empty;

    [Required, MaxLength(100), MinLength(5)]
    public string Subject { get; init; } = string.Empty;

    [Required, MaxLength(5000), MinLength(10)]
    public string Message { get; init; } = string.Empty;

    [Required, MaxLength(50), MinLength(2)]
    public string Name { get; init; } = string.Empty;

    public bool IsValid(out List<ValidationResult> results)
    {
        var context = new ValidationContext(this);
        results = [];
        return Validator.TryValidateObject(this, context, results, true);
    }
}

#endregion

#region Rate Limiting

public static class RateLimitExtensions
{
    public static void AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromSeconds(10),
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

#endregion
