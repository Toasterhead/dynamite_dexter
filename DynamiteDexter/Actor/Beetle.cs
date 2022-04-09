using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Beetle : Pedestrian, IHostile, IOutlined
    {
        private const int SPEED = 1;
        private const uint MIN_DISTANCE = Game1.TILE_SIZE / SPEED;

        private readonly int _startX;
        private readonly int _startY;
        private readonly OutlineMask _outlineMask;

        private uint distanceCount;

        private static readonly Rectangle[] _beetleHitBox =
        {
            new Rectangle(2, 2, Game1.TILE_SIZE - 4, Game1.TILE_SIZE - 4)
        };

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Beetle(int x, int y)
            : base(
                  new SpriteInfo(
                      Images.BEETLE, 
                      x * Game1.TILE_SIZE, 
                      y * Game1.TILE_SIZE, 
                      layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_beetleHitBox, null),
                  new AnimationInfo(2, 4, 4),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _outlineMask = new OutlineMask(Images.BEETLE_OUTLINE, this);
            distanceCount = 0;
            velocity.X = SPEED;
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public override void Update()
        {
            if (distanceCount++ > MIN_DISTANCE && Game1.rand.Next(30) == 0)
            {
                int turn = Game1.rand.Next(2);

                if (velocity.X == 0)
                {
                    if (turn == 0)
                        MoveLeft();
                    else MoveRight();
                }
                else
                {
                    if (turn == 0)
                        MoveUp();
                    else MoveDown();
                }

                distanceCount = 0;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
