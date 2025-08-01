using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace Api.Extensions;

public static class StaticFileExtensions
{
    public static IServiceCollection AddConfiguredStaticFiles(this IServiceCollection services)
    {
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });
        
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        return services;
    }
    
    public static void UseConfiguredStaticFiles(this WebApplication app)
    {
        app.UseResponseCompression();
        
        app.UseStaticFiles(Create());
        
        app.MapFallbackToFile("index.html");
    }
    
    private static StaticFileOptions Create()
    {
        return new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            DefaultContentType = "application/octet-stream",
            OnPrepareResponse = context =>
            {
                const int oneWeek = 604800;
                context.Context.Response.Headers.Append("Cache-Control", $"public, max-age={oneWeek}");
            },
        };
    }
}