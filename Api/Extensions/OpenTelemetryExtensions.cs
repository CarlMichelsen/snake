using Application.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddConfiguredOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddSource(ApplicationConstants.Name)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(ApplicationConstants.Name, serviceVersion: ApplicationConstants.Version))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());

        return services;
    }
}