using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Ghost : SpriteSheet, IHostile, IHunts
    {
        private const int SPEED = 2;
        private const int THRESHOLD = Game1.TILE_SIZE / 4;
        private const uint APPEARING_DURATION = 60;
        private const float DIAGONAL_RATIO = 0.7071f; //Adjacent divided by hypotenuse at 45 degrees.

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;

        private uint appearing;

        private static readonly Rectangle[] _ghostHitBox =
        {
            new Rectangle(3, 2, Game1.TILE_SIZE - 6, Game1.TILE_SIZE * 2 - 4)
        };

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get; set; }

        public Ghost(int x, int y, IGameObject target)
            : base(
                  new SpriteInfo(Images.GHOST, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Overhead),
                  new CollisionInfo(_ghostHitBox, null),
                  new AnimationInfo(2, 4, 12))
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            appearing = APPEARING_DURATION;
        }

        public void Reset() { } //To ensure compliance with IHotile interface.

        protected override void Animate()
        {
            if (appearing == 0)
            {
                render = true;

                if (++delayCount >= interval)
                    delayCount = 0;

                tileSelection.X = (delayCount % 2 == 0 ? 0 : 1);
                tileSelection.Y = (delayCount < interval / 2 ? 0 : 1);

                if (_target.Center.X > Center.X && tileSelection.Y < (sheetDimensions.Y / 2))
                    tileSelection.Y += 2;
                else if (_target.Center.X < Center.X && tileSelection.Y > (sheetDimensions.Y / 2))
                    tileSelection.Y -= 2;

                SetFrame();
            }
            else render = !render;
        }

        public override void Update()
        {
            if (appearing == 0)
            {
                if (_target != null)
                {
                    velocity = new Vector2();

                    if (_target.Center.X < Center.X - THRESHOLD)
                        velocity.X = -SPEED;
                    else if (_target.Center.X > Center.X + THRESHOLD)
                        velocity.X = SPEED;

                    if (_target.Center.Y < Center.Y - THRESHOLD)
                        velocity.Y = -SPEED;
                    else if (_target.Center.Y > Center.Y + THRESHOLD)
                        velocity.Y = SPEED;

                    if (velocity.X != 0.0f && velocity.Y != 0.0f)
                        velocity *= DIAGONAL_RATIO;
                }
            }
            else appearing--;

            base.Update();
        }
    }
}
