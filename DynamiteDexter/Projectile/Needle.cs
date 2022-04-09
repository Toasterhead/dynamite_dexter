using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Needle : SpriteSheet, IProjectile, IOutlined
    {
        private const int SPEED = 5;

        private static readonly Rectangle[] _needleHitBox =
        {
            new Rectangle(0, 0, Game1.SUB_TILE_SIZE, Game1.SUB_TILE_SIZE)
        };

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public bool ContactWithWall { get; set; }
        public bool ContactWithPlayer { get; set; }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }

        public Needle(int x, int y, bool positive, bool vertical, IGameObject parent)
            : base(
                  new SpriteInfo(Images.NEEDLE, 
                      x - (Game1.TILE_SIZE / 2),
                      y - (Game1.TILE_SIZE / 2),
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_needleHitBox, null),
                  new AnimationInfo(4, 4, 1))
        {
            _parent = parent;
            _outlineMask = new OutlineMask(Images.NEEDLE_OUTLINE, this);

            if (vertical && positive)
            {
                velocity = new Vector2(0.0f, SPEED);
                tileSelection.X = 2;
            }
            else if (vertical && !positive)
            {
                velocity = new Vector2(0.0f, -SPEED);
                tileSelection.X = 0;
            }
            else if (!vertical && positive)
            {
                velocity = new Vector2(SPEED, 0.0f);
                tileSelection.X = 1;
            }
            else if (!vertical && !positive)
            {
                velocity = new Vector2(-SPEED, 0.0f);
                tileSelection.X = 3;
            }

            SetFrame();
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, 0); }

        protected override void Animate()
        {
            if (++delayCount >= interval)
            {
                delayCount = 0;

                if (tileSelection.Y < sheetDimensions.Y - 1)
                    tileSelection.Y++;
            }

            SetFrame();
        }
    }
}
