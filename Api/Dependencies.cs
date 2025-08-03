using Api.Extensions;
using Api.Middleware;
using Application.Auth;
using Application.Client.Discord;
using Application.Configuration.Options;
using Application.Snake;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation.Auth;
using Presentation.Client.Discord;
using Presentation.Snake;

namespace Api;

public static class Dependencies
{
    public static void RegisterSnakeDependencies(this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Services
            .AddOpenApi()
            .AddConfiguredOpenTelemetry()
            .AddSingleton(TimeProvider.System);
        builder.Configuration.AddJsonFile(
            "secrets.json",
            optional: builder.Environment.IsDevelopment(),
            reloadOnChange: false);
        builder
            .AddValidatedOptions<SnakeOptions>();
        builder.ApplicationUseSerilog();
        
        // Middleware
        builder.Services
            .AddConfiguredStaticFiles()
            .AddHttpContextAccessor()
            .AddCustomProblemDetails();
        
        // HealthChecks
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        
        // Cache
        builder.Services
            .AddMemoryCache()
            .AddOutputCache();
        
        // Auth
        builder.Services
            .AddSingleton<ISessionContainer, SessionContainer>(); // UserContainer must be a singleton.
        
        // Snake
        builder.Services
            .AddSingleton<ILobbyContainer, LobbyContainer>(); // LobbyContainer must be a singleton.
        
        // SignalR
        builder.Services.AddSignalR();
        
        builder.Services
            .AddScoped<ISimpleLogin, SimpleLogin>()
            .AddScoped<SimpleLoginMiddleware>();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Events.OnRedirectToLogin = (RedirectContext<CookieAuthenticationOptions> context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                
                options.Events.OnRedirectToAccessDenied = (RedirectContext<CookieAuthenticationOptions> context) =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });
        builder.Services.AddAuthorization();
        
        // Client
        builder.Services
            .AddHttpClient<IDiscordWebhookMessageClient, DiscordWebhookMessageClient>()
            .AddStandardResilienceHandler();
        
        // Development
        if (builder.Environment.IsDevelopment())
        {
            builder.Services
                .SetDefaultCorsPolicyToAllowUri("http://localhost:5101");
        }
    }
}