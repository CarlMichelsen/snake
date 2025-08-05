namespace GarrysMod.Model.Dto;

public record ImageResponseDto(
    Guid ImageId,
    SizeDto Size,
    Uri ImageUrl);