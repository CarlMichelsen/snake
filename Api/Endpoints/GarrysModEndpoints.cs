using GarrysMod.Interface;
using GarrysMod.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class GarrysModEndpoints
{
    public static IEndpointRouteBuilder MapGarrysModEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var gmodGroup = endpoints.MapGroup("gmod");

        gmodGroup.MapPost("RequestImage", ([FromServices] IGarrysModImageHandler handler, [FromBody] ImageRequestDto requestDto) =>
            handler.RequestImage(requestDto));
        
        gmodGroup.MapGet("RequestImage", ([FromServices] IGarrysModImageHandler handler, [AsParameters] ImageRequestDto requestDto) =>
            handler.RequestImage(requestDto));

        gmodGroup.MapGet("GetImage/{imageId}", ([FromServices] IGarrysModImageHandler handler, [FromRoute] Guid imageId) =>
            handler.GetImage(imageId: imageId));
        
        gmodGroup.MapGet("GetImage/{imageId}/{chunk}", ([FromServices] IGarrysModImageHandler handler, [FromRoute] Guid imageId, [FromRoute] int chunk) =>
            handler.GetImageChunk(imageId, chunk));

        return gmodGroup;
    }
}