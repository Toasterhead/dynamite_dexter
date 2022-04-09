using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class LiveFlame : Pedestrian, IHostile, IOutlined
    {
        private enum Directions { Left = 0, Up, Right, Down, EnumSize }

        private static Rectangle[] _liveFlameHitBox =
        {
            new Rectangle(2, 7, 12, 8)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly OutlineMask _outlineMask;

        private bool facingRight;
        private Directions currentDirection;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public LiveFlame(int x, int y)
            : base(
                  new SpriteInfo(Images.LIVE_FLAME, x, y, (int)Game1.Layers.Actor),
                  new CollisionInfo(_liveFlameHitBox, null),
                  new AnimationInfo(4, 2, 2),
                  speed: 1)
        {
            _startX = x;
            _startY = y;
            _outlineMask = new OutlineMask(Images.LIVE_FLAME_OUTLINE, this);
            facingRight = true;
            currentDirection = (Directions)Game1.rand.Next((int)Directions.EnumSize);
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        protected override void Animate()
        {
            if (facingRight)
                tileSelection.Y = 0;
            else tileSelection.Y = 1;

            if (++delayCount >= interval)
            {
                if (++tileSelection.X >= sheetDimensions.X)
                    tileSelection.X = 0;

                delayCount = 0;
            }

            SetFrame();
        }

        public override void Reverse()
        {
            base.Reverse();

            int intDirection = (int)currentDirection;
            int intEnumSize = (int)Directions.EnumSize;
            int intNewDirection = (intDirection + 2) % intEnumSize;

            currentDirection = (Directions)intNewDirection;

            if (currentDirection == Directions.Left)
                facingRight = false;
            else if (currentDirection == Directions.Right)
                facingRight = true;
        }

        public override void Update()
        {
            if (Game1.rand.Next(30) == 0)
            { 
                currentDirection = (Directions)Game1.rand.Next((int)Directions.EnumSize + 1);

                //Note - Directions.EnumSize is effectively no direction.

                if (currentDirection == Directions.Left)
                    facingRight = false;
                else if (currentDirection == Directions.Right)
                    facingRight = true;

                velocity = new Vector2();
            }

            switch (currentDirection)
            {
                case Directions.Left:
                    if (velocity.X < 0)
                        velocity.X = 0;
                    else velocity.X = -1;
                    break;
                case Directions.Up:
                    if (velocity.Y < 0)
                        velocity.Y = 0;
                    else velocity.Y = -1;
                    break;
                case Directions.Right:
                    if (velocity.X > 0)
                        velocity.X = 0;
                    else velocity.X = 1;
                    break;
                case Directions.Down:
                    if (velocity.Y > 0)
                        velocity.Y = 0;
                    else velocity.Y = 1;
                    break;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
