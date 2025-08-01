namespace Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection SetDefaultCorsPolicyToAllowUri(this IServiceCollection services, string uri)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy
                    .WithOrigins(uri)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

        return services;
    }
}