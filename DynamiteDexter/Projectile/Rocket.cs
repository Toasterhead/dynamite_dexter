using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Rocket : SpriteSheet, IProjectile, IOutlined
    {
        private const int SPEED = 4;

        private static readonly Rectangle[] _rocketHitBoxHorizontal =
        {
            new Rectangle(1, 4, Game1.TILE_SIZE - 2, 8)
        };
        private static readonly Rectangle[] _rocketHitBoxVertical =
        {
            new Rectangle(4, 1, 8, Game1.TILE_SIZE - 2)
        };

        private readonly OutlineMask _outlineMask;
        private readonly IGameObject _parent;

        public bool ContactWithWall { get; set; }
        public bool ContactWithPlayer { get; set; }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IGameObject Parent { get { return _parent; } }

        public Rocket(int x, int y, bool negative, bool horizontal, IGameObject parent)
            : base(
                  new SpriteInfo(Images.ROCKET, x - (Game1.TILE_SIZE / 2), y - (Game1.TILE_SIZE / 2), (int)Game1.Layers.Actor),
                  new CollisionInfo(horizontal ? _rocketHitBoxHorizontal : _rocketHitBoxVertical, null),
                  new AnimationInfo(4, 1, 0))
        {
            _parent = parent;
            _outlineMask = new OutlineMask(Images.ROCKET_OUTLINE, this);

            if (horizontal)
            {
                if (negative)
                {
                    velocity = new Vector2(-SPEED, 0.0f);
                    tileSelection.X = 0;
                }
                else
                {
                    velocity = new Vector2(SPEED, 0.0f);
                    tileSelection.X = 2;
                }
            }
            else
            {
                if (negative)
                {
                    velocity = new Vector2(0.0f, -SPEED);
                    tileSelection.X = 1;
                }
                else
                {
                    velocity = new Vector2(0.0f, SPEED);
                    tileSelection.X = 3;
                }
            }

            SetFrame();
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, 0); }

        protected override void Animate() { }
    }
}
