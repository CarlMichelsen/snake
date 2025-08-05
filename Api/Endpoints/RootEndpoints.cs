using Api.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class RootEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<LobbyHub>("/lobby");
        
        endpoints.MapHealthChecks("/health");
        
        var apiGroup = endpoints.MapGroup("api/v1").WithParameterValidation();

        apiGroup.MapAuthEndpoints();
        
        apiGroup.MapGarrysModEndpoints();
        
        apiGroup.MapGet("/", ([FromServices] ILogger<Program> logger) => "Hello World!").RequireAuthorization();

        return endpoints;
    }
}