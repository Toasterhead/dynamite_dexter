using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Tank : Pedestrian, IHostile, IFires, IHunts, IHasAttachments, IOutlined //Under construction.
    {
        private class TankBarrel : SpriteSheet
        {
            public TankBarrel(int x, int y)
                : base(
                    new SpriteInfo(Images.TANK_BARREL, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Overhead),
                    new CollisionInfo(null, null),
                    new AnimationInfo(4, 1, 0)) { }

            public void DetermineFrame(int tileSelectionX) { tileSelection.X = tileSelectionX; }

            protected override void Animate() { SetFrame(); }

        }

        private const int SPEED = 1;
        private const uint FIRE_INTERVAL = 60;

        private static readonly Rectangle[] _tankHitBox =
        {
            new Rectangle(1, 1, 2 * Game1.TILE_SIZE - 2, 2 * Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly TankBarrel _barrel;
        private readonly OutlineMask _outlineMask;

        private bool horizontal;
        private uint fireCount;
        private IProjectile[] chamber;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_barrel);

                return attachments;
            }
        }

        public Tank(int x, int y, IGameObject target)
		        : base(
			        new SpriteInfo(Images.TANK, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Actor),
			        new CollisionInfo(_tankHitBox, null),
			        new AnimationInfo(3, 2, 4),
			    speed: SPEED)
	    {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _barrel = new TankBarrel(Center.X - (Game1.TILE_SIZE / 2), Center.Y - (Game1.TILE_SIZE / 2));
            _outlineMask = new OutlineMask(Images.TANK_OUTLINE, this);
            fireCount = FIRE_INTERVAL;
            horizontal = _target.Y > Game1.TILE_SIZE && _target.Y < Game1.playfield.Y - Game1.TILE_SIZE;
            if (horizontal)
                velocity = new Vector2(-SPEED, 0.0f);
            else velocity = new Vector2(0.0f, SPEED);
        }

        public void EmptyChamber() { chamber = null; }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Reset()
        {
            x = _startX;
            y = _startY;
            fireCount = FIRE_INTERVAL;
            horizontal = _target.Y > Game1.TILE_SIZE && _target.Y < Game1.playfield.Y - Game1.TILE_SIZE;
            if (horizontal)
                velocity = new Vector2(-speed, 0.0f);
            else velocity = new Vector2(0.0f, speed);
        }

        public void RepositionBarrel() { _barrel.Reposition(Center.X, Center.Y, fromCenter: true); }

        protected override void Animate()
        {
            if (horizontal && _target.Center.Y >= Center.Y)
                _barrel.DetermineFrame(1);
            else if (horizontal && _target.Center.Y < Center.Y)
                _barrel.DetermineFrame(3);
            else if (!horizontal && _target.Center.X >= Center.X)
                _barrel.DetermineFrame(0);
            else _barrel.DetermineFrame(2);

            _barrel.Reposition(Center.X, Center.Y, fromCenter: true);

            if (++delayCount == interval)
            {
                delayCount = 0;

                if (velocity.X < 0.0f || velocity.Y < 0.0f)
                {
                    if (--tileSelection.X < 0)
                        tileSelection.X = sheetDimensions.X - 1;
                }
                else if (++tileSelection.X > sheetDimensions.X - 1)

                    tileSelection.X = 0;
            }

            if (horizontal)
                tileSelection.Y = 1;
            else tileSelection.Y = 0;

            SetFrame();
        }

        public override void Update()
        {
            if (--fireCount == 0)
            {
                chamber = new IProjectile[1];

                //Modify Rocket class. New constructor will take the following form:
                //Rocket(int x, int y, bool positive, bool horizontal, IGameObject parent)

                //Temporarily using Bullet objects.

                if (horizontal && _target.Center.Y < Center.Y)
                    chamber[0] = new Rocket(_barrel.Center.X, _barrel.Top, true, false, this);
                else if (horizontal && _target.Center.Y >= Center.Y)
                    chamber[0] = new Rocket(_barrel.Center.X, _barrel.Bottom - 1, false, false, this);
                else if (!horizontal && _target.Center.X < Center.X)
                    chamber[0] = new Rocket(_barrel.Left, _barrel.Center.Y, true, true, this);
                else chamber[0] = new Rocket(_barrel.Right - 1, _barrel.Center.Y, false, true, this);

                fireCount = FIRE_INTERVAL;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
