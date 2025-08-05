using GarrysMod.Model;

namespace GarrysMod;

public static class GarrysModImageHolder
{
    private const int MaxImages = 20;
    
    private static List<GarrysModImage> images = [];

    public static void AddImage(GarrysModImage newImage)
    {
        lock (images)
        {
            var existing = images.FirstOrDefault(x => x.ImageId == newImage.ImageId);
            if (existing is not null)
            {
                return;
            }
            
            images.Add(newImage);

            if (images.Count > MaxImages)
            {
                images.RemoveAt(0);
            }
        }
    }

    public static GarrysModImage? GetImage(Guid imageId)
    {
        lock (images)
        {
            return images.FirstOrDefault(x => x.ImageId == imageId);
        }
    }
}