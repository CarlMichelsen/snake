namespace GarrysMod;

using GarrysMod.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class GarrysModDependencyExtensions
{
    public static void RegisterGarrysModDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<ImageService>()
            .AddScoped<IGarrysModImageHandler, GarrysModImageHandler>();
    }
}