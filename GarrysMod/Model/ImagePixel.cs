namespace GarrysMod.Model;

public record struct ImagePixel(
    byte Red,
    byte Green,
    byte Blue)
{
    public override string ToString() =>
        $"{this.Red:X2}{this.Green:X2}{this.Blue:X2}";
}