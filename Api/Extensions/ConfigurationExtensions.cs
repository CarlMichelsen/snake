using Presentation.Configuration.Options;

namespace Api.Extensions;

public static class ConfigurationExtensions
{
    public static IHostApplicationBuilder AddValidatedOptions<TOptions>(this IHostApplicationBuilder builder)
        where TOptions : class, IConfigurationOptions
    {
        builder.Services
            .AddOptions<TOptions>()
            .Bind(builder.Configuration.GetSection(TOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }
}