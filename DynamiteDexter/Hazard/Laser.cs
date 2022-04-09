using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Laser : TileSheet
    {
        private const int INTERVAL = 4 * Game1.SYNC_QUARTER;

        private static readonly Rectangle[] _laserHitBoxVertical =
        {
            new Rectangle(5, 0, 6, Game1.TILE_SIZE)
        };
        private static readonly Rectangle[] _laserHitBoxHorizontal =
        {
            new Rectangle(0, 5, Game1.TILE_SIZE, 6)
        };

        private readonly bool _offset;

        public bool Potent
        {
            get
            {
                if (_offset)

                    return Game1.universalTimer % INTERVAL >= INTERVAL / 2;

                return Game1.universalTimer % INTERVAL < INTERVAL / 2;
            }
        }

        public Laser(int x, int y, bool horizontal, bool offset)
            : base(
                  new SpriteInfo(Images.LASER, x, y, (int)Game1.Layers.Floor),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(horizontal ? _laserHitBoxHorizontal : _laserHitBoxVertical, null),
                  new AnimationInfo(4, 2, 0))
        {
            _offset = offset;

            if (horizontal)
                tileSelection.Y = 0;
            else tileSelection.Y = 1;
        }

        protected override void Animate()
        {
            if (Potent)
            {
                render = true;

                if (++tileSelection.X >= sheetDimensions.X)
                    tileSelection.X = 0;

                SetFrame();
            }
            else render = false;
        }
    }
}
