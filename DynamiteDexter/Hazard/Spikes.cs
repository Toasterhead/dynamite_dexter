namespace DynamiteDexter
{
    public class Spikes : Tile
    {
        public Spikes(int x, int y)
            : base(Images.SPIKES, x, y, (int)Game1.Layers.Floor)
        { }
    }
}
