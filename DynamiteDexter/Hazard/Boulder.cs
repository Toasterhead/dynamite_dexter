using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Boulder : SpriteSheet, IHostile, IHunts
    {
        private const int SPEED = 3;

        private static readonly Rectangle[] _boulderHitBox =
        {
            new Rectangle(1, 1, (Game1.TILE_SIZE * 2) - 2, (Game1.TILE_SIZE * 2) - 2)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly int[] _path;
        private readonly IGameObject _target;
        private readonly Rectangle _tripZone;

        private bool rolling;
        private int pathIndex;
        private int displacementX;
        private int displacementY;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }

        public Boulder(int x, int y, Rectangle tripZone, int[] path, IGameObject target)
            : base(
                  new SpriteInfo(Images.BOULDER, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_boulderHitBox, null),
                  new AnimationInfo(2, 1, 4))
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            _tripZone = new Rectangle(
                tripZone.X * Game1.TILE_SIZE, 
                tripZone.Y * Game1.TILE_SIZE, 
                tripZone.Width * Game1.TILE_SIZE, 
                tripZone.Height * Game1.TILE_SIZE);
            _path = path;
            pathIndex = 0;
            displacementX = 0;
            displacementY = path == null ? 0 : _path[0];
            rolling = false;
            render = false;
        }

        public void Reset()
        {
            pathIndex = 0;
            displacementX = 0;
            displacementY = _path == null ? 0 : _path[0];
            rolling = false;
            render = false;
            velocity = new Vector2();
        }

        public override void Update()
        {
            if (_path != null && !rolling && _target.Rect.Intersects(_tripZone))
            {
                rolling = true;
                render = true;
            }
            else if (rolling)
            {
                if (pathIndex % 2 == 0)
                {
                    int absoluteY = _startY + (displacementY * Game1.TILE_SIZE);

                    bool waypointReached =
                        _path[pathIndex] < 0 ?
                        y <= absoluteY :
                        y >= absoluteY;

                    if (waypointReached && pathIndex < _path.Length - 1)
                    {
                        y = absoluteY;

                        if (pathIndex + 1 < _path.Length)
                            displacementX += _path[++pathIndex];

                        velocity = new Vector2(_path[pathIndex] < 0 ? -SPEED : SPEED, 0.0f);
                    }
                    else velocity = new Vector2(0.0f, _path[pathIndex] < 0 ? -SPEED : SPEED);
                }
                else
                {
                    int absoluteX = _startX + (displacementX * Game1.TILE_SIZE);

                    bool waypointReached =
                        _path[pathIndex] < 0 ?
                        x <= absoluteX :
                        x >= absoluteX;

                    if (waypointReached && pathIndex < _path.Length - 1)
                    {
                        x = absoluteX;

                        if (pathIndex + 1 < _path.Length)
                            displacementY += _path[++pathIndex];

                        velocity = new Vector2(0.0f, _path[pathIndex] < 0 ? -SPEED : SPEED);
                    }
                    else velocity = new Vector2(_path[pathIndex] < 0 ? -SPEED : SPEED, 0.0f);
                }
            }

            base.Update();
        }
    }
}
