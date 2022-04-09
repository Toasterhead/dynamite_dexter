using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Porcupine : Pedestrian, IHostile, IFires, IOutlined
    {
        private enum Directions { Left, Up, Right, Down, EnumSize }

        private const uint DISCHARGE_TIME = 40;
        private const uint MINIMUM_WALK_TIME = 90;

        private static Rectangle[] _porcupineHitBox =
        {
            new Rectangle(1, 7, 14, 9)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly OutlineMask _outlineMask;

        private bool walking;
        private uint count;
        private IProjectile[] chamber;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Porcupine(int x, int y)
            : base(
                  new SpriteInfo(Images.PORCUPINE, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_porcupineHitBox, null),
                  new AnimationInfo(2, 4, 4),
                  speed: 1)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _outlineMask = new OutlineMask(Images.PORCUPINE_OUTLINE, this);
            walking = true;
            count = 0;

            SetWalk();
        }

        public void EmptyChamber() { chamber = null; }

        private void SetWalk()
        {
            Directions direction;

            if (Velocity.X != 0.0f)
                direction = Game1.rand.Next(2) == 0 ? Directions.Up : Directions.Down;
            else if (Velocity.Y != 0.0f)
                direction = Game1.rand.Next(2) == 0 ? Directions.Left : Directions.Right;
            else direction = (Directions)Game1.rand.Next((int)Directions.EnumSize);          

            switch (direction)
            {
                case Directions.Left:
                    MoveLeft();
                    break;
                case Directions.Up:
                    MoveUp();
                    break;
                case Directions.Right:
                    MoveRight();
                    break;
                case Directions.Down:
                    MoveDown();
                    break;
            }
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Reset()
        {
            walking = true;
            count = 0;

            SetWalk();
        }

        protected override void Animate()
        {
            if (!walking)
            {
                tileSelection.X = 0;
                tileSelection.Y = 3;
                SetFrame();
            }
            else base.Animate();
        }

        public override void Update()
        {
            if (walking)
            {
                if (count++ >= MINIMUM_WALK_TIME && Game1.rand.Next(30) == 0)
                {
                    if (Game1.rand.Next(3) > 0)
                    {
                        walking = false;
                        Neutral();
                    }
                    else SetWalk();

                    count = 0;
                }
            }
            else
            {
                if (count++ > DISCHARGE_TIME)
                {
                    walking = true;
                    count = 0;

                    SetWalk();
                }
                else if (count == DISCHARGE_TIME / 2)
                {
                    chamber = new IProjectile[4];
                    chamber[0] = new Needle(Right, Center.Y, true, false, this);
                    chamber[1] = new Needle(Left, Center.Y, false, false, this);
                    chamber[2] = new Needle(Center.X, Bottom, true, true, this);
                    chamber[3] = new Needle(Center.X, Top, false, true, this);
                }
            }

            BoundsCheck();
            base.Update();
        }
    }
}
