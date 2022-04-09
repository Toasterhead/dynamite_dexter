using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Robot : Pedestrian, IHostile, IHunts, IOutlined
    {
        private const int SPEED = 2;

        private static readonly Rectangle[] _robotHitBox =
        {
            new Rectangle(4, 2, Game1.TILE_SIZE - 8, Game1.TILE_SIZE - 3)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly OutlineMask _outlineMask;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Robot(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(Images.ROBOT, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_robotHitBox, null),
                  new AnimationInfo(3, 4, 2),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _outlineMask = new OutlineMask(Images.ROBOT_OUTLINE, this);

            //Using convoluted formulae to give the appearance of facing a random direction.
            tileSelection.X = (7 * y) % sheetDimensions.X;
            tileSelection.Y = (x * y) % sheetDimensions.Y;
            SetFrame();
        }

        public override void Reset() { Game1.PlaySound(Sounds.ROBOT); }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        protected override void Animate()
        {
            if (velocity.X != 0.0f || velocity.Y != 0.0f)
            {
                if (velocity.X < 0.0f)
                    tileSelection.Y = 0;
                else if (velocity.Y < 0.0f)
                    tileSelection.Y = 2;
                else if (velocity.X > 0.0f)
                    tileSelection.Y = 1;
                else //if (velocity.Y > 0.0f)
                    tileSelection.Y = 3;

                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= sheetDimensions.X)
                        tileSelection.X = 0;

                    delayCount = 0;
                }

                SetFrame();
            }
        }

        public override void Update()
        {
            if (_target.Center.X >= Left && _target.Center.X < Right)
            {
                if (_target.Center.Y < Center.Y)
                    MoveUp();
                else MoveDown();
            }
            else if (_target.Center.Y >= Top && _target.Center.Y < Bottom)
            {
                if (_target.Center.X < Center.X)
                    MoveLeft();
                else MoveRight();
            }
            else Neutral();

            base.Update();
        }
    }
}
