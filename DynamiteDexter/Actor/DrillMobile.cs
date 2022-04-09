using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class DrillMobile : Pedestrian, IHostile, IDestructive
    {
        public const int SPEED = 1;
        private const uint MINIMUM_TRAVEL = 30;

        private static readonly Rectangle[] _crabHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;

        private uint travelCount;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }

        public DrillMobile(int x, int y)
            : base(
                  new SpriteInfo(Images.DRILL_MOBILE, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_crabHitBox, null),
                  new AnimationInfo(3, 4, 2),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            travelCount = 0;

            switch (Game1.rand.Next(4))
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

        public override void Reset() { travelCount = 0; }

        protected override void Animate()
        {
            if (velocity.X < 0)
                tileSelection.Y = 0;
            else if (velocity.X > 0)
                tileSelection.Y = 1;
            else if (velocity.Y < 0)
                tileSelection.Y = 2;
            else //if (velocity.Y > 0)
                tileSelection.Y = 3;

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
            if (++travelCount >= MINIMUM_TRAVEL && Game1.rand.Next(60) == 0)
            {
                if (velocity.X != 0.0f)
                {
                    if (Game1.rand.Next(2) == 0)
                        MoveUp();
                    else MoveDown();
                }
                else if (velocity.Y != 0.0f)
                {
                    if (Game1.rand.Next(2) == 0)
                        MoveLeft();
                    else MoveRight();
                }

                travelCount = 0;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
