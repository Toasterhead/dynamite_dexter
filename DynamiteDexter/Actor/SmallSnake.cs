using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class SmallSnake : TileSheet, IHostile, IHunts, IHasAttachments
    {
        private class Head : SpriteSheet
        {
            public Head(int x, int y)
                : base(
                      new SpriteInfo(Images.SMALL_SNAKE_HEAD, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, (int)Game1.Layers.Floor),
                      new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                      new CollisionInfo(null, null),
                      new AnimationInfo(5, 4, 0)) { }

            public void DetermineFrame(int tileSelectionX, int tileSelectionY)
            {
                tileSelection.X = tileSelectionX;
                tileSelection.Y = tileSelectionY;
            }

            protected override void Animate() { SetFrame(); }
        }

        private const uint STRIKE_DURATION = 60;
        const uint FULL_ATTACK_CYCLE = 2 * STRIKE_DURATION;

        private static readonly Rectangle[] _smallSnakeHitBox =
        {
            new Rectangle(0, 0, 0, 0),
            new Rectangle(-28, 4, 42, 8),
            new Rectangle(2, -12, 12, 24),
            new Rectangle(2, 4, 42, 8)
        };
        private static readonly int[] tileSelectionRemap = { 1, 2, 3, 2 };

        private readonly int _startX;
        private readonly int _startY;
        private readonly Rectangle _detectionArea;
        private readonly IGameObject _target;
        private readonly Head _head;

        private int tileSelectionStrike;
        private uint strikeModeCount;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.Add(_head as SpriteSheet);

                return attachments;
            }
        }
        public Rectangle HitBoxAssault
        {
            get
            {
                if (tileSelection.Y == 2 && tileSelection.X > 0)
                    return GetHitBox(1);
                else if (tileSelection.Y == 3 && tileSelection.X > 0)
                    return GetHitBox(3);
                else if (tileSelection.Y == 0 || tileSelection.Y == 1)
                    return GetHitBox(2);

                return GetHitBox(0);
            }
        }

        public SmallSnake(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(Images.SMALL_SNAKE_COIL, x, y, (int)Game1.Layers.Floor),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(_smallSnakeHitBox, null),
                  new AnimationInfo(4, 4, 5))
        {
            _startX = x;
            _startY = y;
            _target = target;
            _head = new Head(x - 1, y - 1);
            _detectionArea = new Rectangle(
                (x - 3) * Game1.TILE_SIZE, 
                (int)(y - 1.5f) * Game1.TILE_SIZE,
                7 * Game1.TILE_SIZE, 
                4 * Game1.TILE_SIZE);
            strikeModeCount = 0;
        }

        public void Reset() { strikeModeCount = 0; }

        protected override void Animate()
        {
            bool targetToLeft = _target.Center.X < Center.X;
            uint adjustedCount = strikeModeCount % FULL_ATTACK_CYCLE;
            uint halfInterval = interval / 2;

            if (strikeModeCount == 0 || adjustedCount >= STRIKE_DURATION)
            {
                if (targetToLeft)
                {
                    tileSelection.Y = 0;
                    _head.Reposition(X - Game1.TILE_SIZE, Y - Game1.TILE_SIZE);
                }
                else
                {
                    tileSelection.Y = 1;
                    _head.Reposition(X, Y - Game1.TILE_SIZE);
                }

                if (++delayCount >= interval)
                {
                    if (++tileSelection.X >= sheetDimensions.X)
                        tileSelection.X = 0;

                    delayCount = 0;
                }

                if (tileSelection.X == 0 && Game1.rand.Next((int)interval) == 0) //Randomly stick out tongue.
                    _head.DetermineFrame(_head.SheetDimensions.X - 1, tileSelection.Y);
                else _head.DetermineFrame(tileSelection.X, tileSelection.Y);
            }
            else
            {
                if (_target.Center.X < Center.X)
                    tileSelection.Y = 2;
                else tileSelection.Y = 3;

                if (adjustedCount < halfInterval || 
                    adjustedCount >= STRIKE_DURATION - halfInterval && adjustedCount < STRIKE_DURATION)

                    tileSelection.X = 0;

                else if (++delayCount >= halfInterval)
                {
                    if (tileSelection.X == 0)
                        tileSelection.X = 1;

                    if (++tileSelectionStrike >= sheetDimensions.X)
                        tileSelectionStrike = 0;

                    tileSelection.X = tileSelectionRemap[tileSelectionStrike];

                    delayCount = 0;
                }

                _head.DetermineFrame(tileSelection.X, tileSelection.Y);

                if (tileSelection.X > 0)
                    _head.Reposition(targetToLeft ? X - (2 * Game1.TILE_SIZE) : X + Game1.TILE_SIZE, Y);
                else _head.Reposition(targetToLeft ? X - Game1.TILE_SIZE : X, Y - Game1.TILE_SIZE);
            }

            SetFrame();
        }

        public override void Update()
        {
            if (_target.Rect.Intersects(_detectionArea))
                strikeModeCount++;
            else if (strikeModeCount > 0 && strikeModeCount % FULL_ATTACK_CYCLE < STRIKE_DURATION)
                strikeModeCount++;
            else strikeModeCount = 0;

            if (strikeModeCount == 1)
                Game1.PlaySound(Sounds.SNAKE);

            base.Update();
        }
    }
}
