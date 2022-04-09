using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class DynamiteIgnited : SpriteSheet
    {
        private const int START_TIME = 45;

        private static readonly Rectangle[] _dynamiteIgnitedHitBox =
        {
            new Rectangle(6, 2, Game1.TILE_SIZE - 12, Game1.TILE_SIZE - 4)
        };

        private int count;

        public bool TimeUp { get { return count <= 0; } } 
        public int Count { get { return count; } }
        public Rectangle PlacementCollision { get { return GetHitBox(0); } }

        public DynamiteIgnited(int x, int y)
            : base(
                  new SpriteInfo(
                      Images.DYNAMITE_IGNITED, 
                      x - (int)(Tile.TileSize / 2), 
                      y - (int)(Tile.TileSize / 2), 
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_dynamiteIgnitedHitBox, null),
                  new AnimationInfo(2, 1, 2))
        { count = START_TIME; }

        public override void Update()
        {
            count--;
            base.Update();
        }
    }
}
