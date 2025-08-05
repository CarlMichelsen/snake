using GarrysMod.Model;
using GarrysMod.Model.Dto;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GarrysMod;

public class ImageService(HttpClient httpClient)
{
    public async Task<GarrysModImage?> GetImage(ImageRequestDto imageRequest)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageRequest.ImageUrl);
        
        using var image = Image.Load<Rgb24>(imageBytes);
        
        var scale = Math.Min((float)imageRequest.SquareResolution / image.Width, (float)imageRequest.SquareResolution / image.Height);
        var newWidth = (int)(image.Width * scale);
        var newHeight = (int)(image.Height * scale);
        
        image.Mutate(x => x.Resize(newWidth, newHeight));
        var pixels = new List<ImagePixel>();
        
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (int x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    pixels.Add(new ImagePixel(pixel.R, pixel.G, pixel.B));
                }
            }
        });

        return new GarrysModImage
        {
            ImageXSize = newWidth,
            ImageYSize = newHeight,
            Source = imageRequest.ImageUrl,
            Pixels = pixels.ToArray(),
        };
    }
}