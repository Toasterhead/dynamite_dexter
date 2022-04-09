using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class Alligator : TileSheet
    {
        private const int NUM_QUARTERS = 8; //Half-time.

        private static readonly Rectangle[] _alligatorHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private readonly bool _offset;

        private uint count;

        public bool Lethal
        {
            get
            {
                int count = Game1.universalTimer % (NUM_QUARTERS * Game1.SYNC_QUARTER);
                int checkQuarter = _offset ? 2 : 0;

                return count / (2 * Game1.SYNC_QUARTER) == checkQuarter;
            }
        }

        public Alligator(int x, int y, bool offset = false)
            : base(
                  new SpriteInfo(Images.ALLIGATOR, x, y, (int)Game1.Layers.Floor),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(_alligatorHitBox, null),
                  new AnimationInfo(2, 1, 0))
        {
            _offset = offset;
            count = 0;
        }

        protected override void Animate()
        {
            tileSelection.X = Lethal ? 1 : 0;
            SetFrame();
        }

        public override void Update()
        {
            count++;
            base.Update();
        }
    }
}
