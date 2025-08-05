namespace GarrysMod.Model;

public class GarrysModImage
{
    public Guid ImageId { get; } = Guid.NewGuid();
    
    public required int ImageXSize { get; init; }
    
    public required int ImageYSize { get; init; }
    
    public required Uri Source { get; init; }
    
    public required ImagePixel[] Pixels { get; init; } = [];
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}