using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Crab : Pedestrian, IHostile, IHunts, IOutlined
    {
        private const int SPEED = 1;
        private const uint MINIMUM_WALK = 30;

        private static readonly Rectangle[] _crabHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly OutlineMask _outlineMask;

        private uint walkCount;

        public bool Submerged { get; set; }
        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Crab(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(Images.CRAB, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_crabHitBox, null),
                  new AnimationInfo(3, 2, 4),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _outlineMask = new OutlineMask(Images.CRAB_OUTLINE, this);
            Submerged = false;
            walkCount = 0;

            int direction = Game1.rand.Next(4);

            switch(direction)
            {
                case 0: MoveLeft();
                    break;
                case 1: MoveUp();
                    break;
                case 2: MoveRight();
                    break;
                case 3: MoveDown();
                    break;
            }
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Reset()
        {
            Submerged = false;
            walkCount = 0;
        }

        protected override void Animate()
        {
            if (!Submerged)
            {
                tileSelection.Y = 0;

                if (++delayCount >= interval)
                {
                    if (tileSelection.X == 0)
                        tileSelection.X = 1;
                    else tileSelection.X = 0;

                    delayCount = 0;
                }
            }
            else
            {
                tileSelection.Y = 1;

                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= sheetDimensions.X)
                        tileSelection.X = 0;

                    delayCount = 0;
                }
            }

            SetFrame();
        }

        public override void Update()
        {
            if (++walkCount >= MINIMUM_WALK && Game1.rand.Next(30) == 0)
            {
                if (velocity.X != 0.0f)
                {
                    if (_target.Center.Y < Center.Y)
                        MoveUp();
                    else MoveDown();
                }
                else if (velocity.Y != 0.0f)
                {
                    if (_target.Center.X < Center.X)
                        MoveLeft();
                    else MoveRight();
                }

                walkCount = 0;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
