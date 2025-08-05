namespace GarrysMod.Interface;

using GarrysMod.Model.Dto;

public interface IGarrysModImageHandler
{
    Task<ImageResponseDto> RequestImage(ImageRequestDto imageRequest);

    Task<ImageResponseDto> GetImage(Guid imageId);

    Task<string> GetImageChunk(Guid imageId, int chunk);
}