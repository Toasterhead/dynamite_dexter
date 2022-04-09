using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Cloud : SpriteSheet, IHostile, IHunts, INavigates, IHasAttachments, IOutlined
    {
        private const int HAPPY_SPEED = 1;
        private const int ANGRY_SPEED = 2;
        private const int THRESHOLD = Game1.TILE_SIZE / 4;
        private const float DIAGONAL_RATIO = 0.7071f; //Adjacent divided by hypotenuse at 45 degrees.
        private const uint MIN_DISTANCE = (3 * Game1.TILE_SIZE) / HAPPY_SPEED;
        private const uint LIGHTNING_STRIKE_TIME = 15;
        private const uint _FLASH_INTERVAL = 6;

        private static Point playfield = Game1.playfield;

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;
        private Rectangle _territory;
        private Sprite _lightning;
        private CloudFace _face;
        private List<IGameObject> _spriteSet;

        private static readonly Rectangle[] _cloudHitBox =
        {
            new Rectangle(1, 17, 15, 7)
        };
        private static readonly Point _shadowCenter = new Point(8, 20);

        private readonly OutlineMask _outlineMask;

        private bool happy;
        private bool aboveTerrain;
        private uint distanceCount;
        private uint lightningCount;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public Sprite Lightning { get { return _lightning; } }
        public CloudFace Face { get { return _face; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(Face);
                attachment.Add(Lightning);

                return attachment;
            }
        }

        public Cloud(int x, int y, IGameObject target, Rectangle territory)
            : base(
                  new SpriteInfo(
                      Images.CLOUD_SHADOW, 
                      x * Game1.TILE_SIZE, 
                      y * Game1.TILE_SIZE, 
                      layer: (int)Game1.Layers.Passable),
                  new CollisionInfo(_cloudHitBox, null),
                  new AnimationInfo(7, 3, 2))
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _target = target;
            distanceCount = 0;
            lightningCount = 0;
            velocity.X = HAPPY_SPEED;
            happy = true;
            aboveTerrain = false;
            _territory = new Rectangle(
                territory.X * Game1.TILE_SIZE, 
                territory.Y * Game1.TILE_SIZE, 
                territory.Width * Game1.TILE_SIZE, 
                territory.Height * Game1.TILE_SIZE);
            _face = new CloudFace(Left, Top - Game1.TILE_SIZE);
            _outlineMask = new OutlineMask(Images.CLOUD_OUTLINE, _face);
            _lightning = new Sprite(
                Images.CLOUD_LIGHTNING, 
                Center.X - Images.CLOUD_LIGHTNING.Width / 2, 
                Top, 
                (int)Game1.Layers.Actor);
            _lightning.Render = false;
        }

        public void AcquireSet(List<IGameObject> spriteSet) { _spriteSet = spriteSet; }

        protected override void Animate()
        {
            render = true;

            if (aboveTerrain && happy)
                render = false;
            else if (aboveTerrain && !happy)
                tileSelection.Y = 1;
            else if (happy)
                tileSelection.Y = 2;
            else tileSelection.Y = 0;

            if (lightningCount > 0)
            {
                _lightning.Render = lightningCount % _FLASH_INTERVAL < _FLASH_INTERVAL / 2 ? true : false;
            }
            else _lightning.Render = false;

            if (++delayCount >= interval)
            { 
                if (++tileSelection.X >= sheetDimensions.X)
                    tileSelection.X = 0;

                delayCount = 0;
            }

            _face.Happy = happy;

            SetFrame();
        }

        public void Reset()
        {
            happy = true;
            render = false;
            velocity.Normalize();
            velocity *= HAPPY_SPEED;
            lightningCount = 0;
            _lightning.Render = false;
            _face.Reposition(Left, Top - Game1.TILE_SIZE);
            if (velocity.X != 0.0f && velocity.Y != 0.0f)
                velocity.Y = 0.0f;
        }

        public void SyncOutline() { _outlineMask.Sync(_face.X, _face.Y, _face.TileSelection.X, _face.TileSelection.Y); }

        public override void Update()
        {
            aboveTerrain = false;

            if (_spriteSet != null)

                foreach (IGameObject i in _spriteSet)

                    if (i is Terrain && GetHitBox(0).Intersects(i.Rect))
                    {
                        aboveTerrain = true;
                        break;
                    }

            //Happy State

            if (happy)
            {
                if (_target is Pedestrian && (_target as Pedestrian).HitBoxAssault.Intersects(_territory))
                    happy = false;

                if (distanceCount++ > MIN_DISTANCE && Game1.rand.Next(60) == 0)
                {
                    int turn = Game1.rand.Next(2);

                    if (velocity.X == 0)
                    {
                        if (turn == 0)
                            velocity = new Vector2(-HAPPY_SPEED, 0.0f);
                        else velocity = new Vector2(HAPPY_SPEED, 0.0f);
                    }
                    else
                    {
                        if (turn == 0)
                            velocity = new Vector2(0.0f, -HAPPY_SPEED);
                        else velocity = new Vector2(0.0f, HAPPY_SPEED);
                    }

                    distanceCount = 0;
                }

                bool reverse = false;

                if (x >= playfield.X - Width)
                {
                    x = playfield.X - Width - 1;
                    reverse = true;
                }
                else if (x < 0)
                {
                    x = 0;
                    reverse = true;
                }
                else if (y >= playfield.Y - Height)
                {
                    y = playfield.Y - Height - 1;
                    reverse = true;
                }
                else if (y < 0)
                {
                    y = 0;
                    reverse = true;
                }

                if (reverse)
                    velocity = -velocity;
            }

            //Angry State

            else if (_target != null)
            {
                if (lightningCount == 0 && Game1.rand.Next(15) == 0)
                {
                    double radiusOne = Width * 5;
                    double radiusTwo = _target.Width / 2;

                    double a = _target.Center.X - Center.X;
                    double b = _target.Center.Y - Center.Y;
                    double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                    if (c < radiusOne + radiusTwo)
                        lightningCount = LIGHTNING_STRIKE_TIME;
                }
                else if (lightningCount > 0)
                    lightningCount--;

                velocity = new Vector2();

                if (_target.Center.X < x + _shadowCenter.X - THRESHOLD)
                    velocity.X = -ANGRY_SPEED;
                else if (_target.Center.X > x + _shadowCenter.X + THRESHOLD)
                    velocity.X = ANGRY_SPEED;

                if (_target.Center.Y < y + _shadowCenter.Y - THRESHOLD)
                    velocity.Y = -ANGRY_SPEED;
                else if (_target.Center.Y > y + _shadowCenter.Y + THRESHOLD)
                    velocity.Y = ANGRY_SPEED;

                if (velocity.X != 0.0f && velocity.Y != 0.0f)
                    velocity *= DIAGONAL_RATIO;
            }

            base.Update();

            _lightning.Reposition(Center.X - (_lightning.Width / 2), Top);
            _face.Reposition(Left, Top - Game1.TILE_SIZE);
        }
    }

    public class CloudFace : SpriteSheet
    {
        public bool Happy { get; set; }
        public Point TileSelection { get { return tileSelection; } }

        public CloudFace(int x, int y)
            : base(
                  new SpriteInfo(Images.CLOUD, x, y, layer: (int)Game1.Layers.Overhead),
                  new CollisionInfo(null, null),
                  new AnimationInfo(3, 2, 2)) { }

        protected override void Animate()
        {
            tileSelection.Y = Happy ? 0 : 1;

            if (++delayCount >= interval)
            {
                if (++tileSelection.X >= sheetDimensions.X)
                    tileSelection.X = 0;

                delayCount = 0;
            }

            SetFrame();
        }
    }
}
