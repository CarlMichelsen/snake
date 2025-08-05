namespace GarrysMod.Model.Dto;

public record ImageResponseDto(
    Guid ImageId,
    SizeDto Size,
    int ChunkCount,
    Uri ImageUrl);