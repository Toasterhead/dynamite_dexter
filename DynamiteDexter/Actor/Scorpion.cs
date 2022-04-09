using System;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Scorpion : Pedestrian, IHostile, IHunts, IOutlined
    {
        private const int SPEED = 2;
        private const uint MIN_DISTANCE = Game1.TILE_SIZE / SPEED;
        private const float ZERO_THRESHOLD = 0.01f;

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private readonly OutlineMask _outlineMask;

        private bool movingVertically;
        private uint distanceCount;

        private static readonly Rectangle[] _scorpionHitBox =
        {
            new Rectangle(2, 2, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Scorpion(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(Images.SCORPION, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_scorpionHitBox, null),
                  new AnimationInfo(2, 4, 4),
                  SPEED)
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _outlineMask = new OutlineMask(Images.SCORPION_OUTLINE, this);
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

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
                    if (movingVertically)
                    {
                        if (_target.Center.X < Center.X)
                            MoveLeft();
                        else MoveRight();
                    }
                    else
                    {
                        if (_target.Center.Y < Center.Y)
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
