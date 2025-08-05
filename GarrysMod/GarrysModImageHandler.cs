using System.ComponentModel.DataAnnotations;

namespace GarrysMod;

using GarrysMod.Interface;
using GarrysMod.Model.Dto;

public class GarrysModImageHandler(ImageService imageService) : IGarrysModImageHandler
{
    public async Task<ImageResponseDto> RequestImage(ImageRequestDto imageRequest)
    {
        var image = await imageService.GetImage(imageRequest);
        if (image is null)
        {
            throw new ValidationException("Unable to retrieve image");
        }
        
        GarrysModImageHolder.AddImage(image);
        return new ImageResponseDto(
            ImageId: image.ImageId,
            Size: new SizeDto(image.ImageXSize, image.ImageYSize),
            ImageUrl: image.Source);
    }

    public Task<ImageResponseDto> GetImage(Guid imageId)
    {
        var image = GarrysModImageHolder.GetImage(imageId);
        if (image is null)
        {
            throw new ValidationException("Unable to retrieve image");
        }
        
        return Task.FromResult(new ImageResponseDto(
            ImageId: image.ImageId,
            Size: new SizeDto(image.ImageXSize, image.ImageYSize),
            ImageUrl: image.Source));
    }

    public Task<List<ImageResponseDto>> GetImages()
    {
        return Task.FromResult(GarrysModImageHolder
            .Images
            .Select(i => new ImageResponseDto(
                ImageId: i.ImageId,
                Size: new SizeDto(i.ImageXSize, i.ImageYSize),
                ImageUrl: i.Source))
            .ToList());
    }

    public Task<string> GetImageChunk(Guid imageId, int skip, int take)
    {
        var image = GarrysModImageHolder.GetImage(imageId);
        if (image is null)
        {
            throw new ValidationException("Unable to retrieve image");
        }

        var chunks = image.Pixels
            .Skip(skip)
            .Take(take)
            .Select(c => c.ToString());

        return Task.FromResult(string.Join(',', chunks));
    }
}