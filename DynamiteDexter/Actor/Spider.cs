using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Spider : Pedestrian, IHostile
    {
        private const int SPEED = 2;
        private const uint MIN_DISTANCE = Game1.TILE_SIZE / SPEED;
        private const float ZERO_THRESHOLD = 0.01f;

        private readonly int _startX;
        private readonly int _startY;

        private bool movingVertically;
        private uint distanceCount;

        private static readonly Rectangle[] _spiderHitBox =
        {
            new Rectangle(2, 2, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }

        public Spider(int x, int y)
            : base(
                  new SpriteInfo(Images.SPIDER, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_spiderHitBox, null),
                  new AnimationInfo(2, 4, 4),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
        }

        public override void Update()
        {
            if (distanceCount++ > MIN_DISTANCE && Game1.rand.Next(20) == 0)
            {
                if (velocity.X < -ZERO_THRESHOLD || velocity.X > ZERO_THRESHOLD)
                {
                    movingVertically = false;
                    velocity = new Vector2();
                }
                else if (velocity.Y < -ZERO_THRESHOLD || velocity.Y > ZERO_THRESHOLD)
                {
                    movingVertically = true;
                    velocity = new Vector2();
                }
                else
                {
                    int turn = Game1.rand.Next(2);

                    if (movingVertically)
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
                }

                distanceCount = 0;
            }

            BoundsCheck();
            base.Update();
        }
    }
}
