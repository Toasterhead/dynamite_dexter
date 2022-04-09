using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class DirtDevil : Pedestrian, IHostile, IFires, IDestructive, IOutlined
    {
        private const uint DEBRIS_CYCLE = 120;
        private const uint RELEASE_PER_CYCLE = 4;
        private const int MINIMUM_TRAVEL_TIME = 16;

        private static readonly Rectangle[] _dirtDevilHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;

        private uint travelTime;
        private uint debrisReleaseCount;
        private IProjectile[] chamber;
        private readonly OutlineMask _outlineMask;

        public IProjectile[] Chamber { get { return chamber; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }

        public DirtDevil(int x, int y)
            : base(
                new SpriteInfo(Images.DIRT_DEVIL, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Actor),
                new CollisionInfo(_dirtDevilHitBox, null),
                new AnimationInfo(4, 1, 4),
                speed: 1)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _outlineMask = new OutlineMask(Images.DIRT_DEVIL_OUTLINE, this);
            travelTime = 0;
            debrisReleaseCount = 0;
        }

        public void EmptyChamber() { chamber = null; }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Reset()
        {
            Reposition(_startX, _startY);
            travelTime = 0;
            debrisReleaseCount = 0;
        }

        protected override void Animate()
        {
            if (++delayCount >= interval)
            {
                if (++tileSelection.X >= sheetDimensions.X)
                    tileSelection.X = 0;

                delayCount = 0;
            }

            SetFrame();
        }

        public override void Update()
        {
            const uint SUB_INTERVAL = DEBRIS_CYCLE / RELEASE_PER_CYCLE;

            if (debrisReleaseCount % SUB_INTERVAL == 0)
            {
                chamber = new IProjectile[1];
                uint debrisNum = debrisReleaseCount / SUB_INTERVAL;

                switch (debrisNum)
                {
                    case 0:
                        chamber[0] = new Needle(Center.X, Center.Y, false, false, this);
                        break;
                    case 1:
                        chamber[0] = new Needle(Center.X, Center.Y, false, true, this);
                        break;
                    case 2:
                        chamber[0] = new Needle(Center.X, Center.Y, true, false, this);
                        break;
                    case 3:
                        chamber[0] = new Needle(Center.X, Center.Y, true, true, this);
                        break;
                    default: throw new System.Exception("Error - Ivalid result. Check calculations for dirt devil's debris cycle");
                }
            }

            if (++debrisReleaseCount >= DEBRIS_CYCLE)
                debrisReleaseCount = 0;

            if (++travelTime > MINIMUM_TRAVEL_TIME && Game1.rand.Next(30) == 0)
            {
                int direction = Game1.rand.Next(4);

                switch (direction)
                {
                    case 0:
                        MoveLeft();
                        break;
                    case 1:
                        MoveUp();
                        break;
                    case 2:
                        MoveRight();
                        break;
                    case 3:
                        MoveDown();
                        break;
                }

                travelTime = 0;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
