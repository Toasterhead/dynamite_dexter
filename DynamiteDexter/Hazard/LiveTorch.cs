using Microsoft.Xna.Framework;

namespace DynamiteDexter
{ 
    public class LiveTorch : Trippable
    {
        private static readonly Rectangle _tripZoneRel = new Rectangle(
            0, 
            -Game1.TILE_SIZE,
            4 * Game1.TILE_SIZE,
            3 * Game1.TILE_SIZE);

        public LiveTorch(int x, int y)
            : base(Images.CANDLE_TOP, x, y, 2, 4, _tripZoneRel) { }

        public override IGameObject[] GetItem(IGameObject target)
        {
            IGameObject[] item = new IGameObject[2];
            item[0] = new SurpriseFlame(X, Y, -12, target, this);
            item[1] = (item[0] as SurpriseFlame).Subject;

            return item;
        }
    }
}
