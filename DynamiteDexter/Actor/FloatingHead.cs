using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class FloatingHead : Pedestrian, IHostile, IFires, IOutlined
    {
        private const uint RELOAD_TIME = 30;

        private static Rectangle[] _floatingHeadHitBox =
        {
            new Rectangle(2, 2, Game1.TILE_SIZE - 4, Game1.TILE_SIZE - 4)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly OutlineMask _outlineMask;

        private bool blinking;
        private uint reloadCount;
        private IProjectile[] chamber;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public FloatingHead(int x, int y, bool fast = false)
            : base(
                  new SpriteInfo(Images.FLOATING_HEAD, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Actor),
                  new CollisionInfo(_floatingHeadHitBox, null),
                  new AnimationInfo(4, 1, 2),
                  speed: fast ? 3 : 2)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _outlineMask = new OutlineMask(Images.FLOATING_HEAD_OUTLINE, this);
            blinking = false;
            reloadCount = RELOAD_TIME;
            if (Game1.rand.Next(2) == 0)
                MoveLeft();
            else MoveRight();
        }

        public void EmptyChamber() { chamber = null; }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        protected override void Animate()
        {

            if (blinking && ++delayCount >= interval)
            {
                delayCount = 0;

                if (++tileSelection.X >= SheetDimensions.X)
                { 
                    tileSelection.X = 0;
                    blinking = false;
                }
            }

            SetFrame();
        }

        public override void Update()
        {
            if (ContactWithTerrain)
            {
                if (velocity.X > 0.0f)
                    MoveLeft();
                else MoveRight();
            }

            if (--reloadCount == 0)
            {
                chamber = new IProjectile[1];
                chamber[0] = new Fireball(Center.X, Bottom, true, this);

                reloadCount = RELOAD_TIME;
            }

            if (!blinking && Game1.rand.Next(60) == 0)
                blinking = true;

            base.Update();
        }
    }
}
