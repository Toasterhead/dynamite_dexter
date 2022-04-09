using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Hypnoball : SpriteSheet, IHostile, INavigates, IOutlined
    {
        private static readonly Point playfield = Game1.playfield;
        private static readonly Rectangle[] _hypnoballHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, Game1.TILE_SIZE - 2)
        };
        private static readonly Point[] _hypnoballCollisionPoint =
        {
            new Point(0, Game1.TILE_SIZE / 2),
            new Point(Game1.TILE_SIZE / 2, 0),
            new Point(Game1.TILE_SIZE - 1, Game1.TILE_SIZE / 2),
            new Point(Game1.TILE_SIZE / 2, Game1.TILE_SIZE - 1)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly OutlineMask _outlineMask;

        private List<IGameObject> spriteSet;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public OutlineMask TheOutlineMask { get { return _outlineMask; } }

        public Hypnoball(int x, int y)
            : base(
                  new SpriteInfo(Images.HYPNOBALL, x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_hypnoballHitBox, _hypnoballCollisionPoint),
                  new AnimationInfo(5, 1, 2))
        {
            _startX = x * Game1.TILE_SIZE;
            _startY = y * Game1.TILE_SIZE;
            _outlineMask = new OutlineMask(Images.HYPNOBALL_OUTLINE, this);
            velocity = new Vector2(1, 1);
            spriteSet = null;
        }

        public void AcquireSet(List<IGameObject> spriteSet) { this.spriteSet = spriteSet; }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }

        public void Reset() { velocity = new Vector2(1, 1); }

        public override void Update()
        {
            Point left = GetCollisionPoint(0, absolute: true);
            Point top = GetCollisionPoint(1, absolute: true);
            Point right = GetCollisionPoint(2, absolute: true);
            Point bottom = GetCollisionPoint(3, absolute: true);

            if (spriteSet != null)

                foreach (IGameObject i in spriteSet)

                    if (i is Terrain && !(i is Passable) && !(i is Water) && Rect.Intersects(i.Rect))
                    {
                        if (left.X <= i.Right && left.X > i.Center.X && left.Y >= i.Top && left.Y < i.Bottom && x < previousX)
                        {
                            SetCollisionPoint(0, new Point(i.Right + 1, left.Y), absolute: true);
                            velocity.X = -velocity.X;
                            break;
                        }
                        else if (right.X > i.Left && right.X <= i.Center.X && right.Y >= i.Top && right.Y < i.Bottom && x > previousX)
                        {
                            SetCollisionPoint(2, new Point(i.Left, right.Y), absolute: true);
                            velocity.X = -velocity.X;
                            break;
                        }
                        else if (top.Y <= i.Bottom && top.Y > i.Center.Y && top.X >= i.Left && top.X < i.Right && y < previousY)
                        {
                            SetCollisionPoint(1, new Point(top.X, i.Bottom + 1), absolute: true);
                            velocity.Y = -velocity.Y;
                            break;
                        }
                        else if (bottom.Y > i.Top && bottom.Y <= i.Center.Y && top.X >= i.Left && top.X < i.Right && y > previousY)
                        {
                            SetCollisionPoint(3, new Point(bottom.X, i.Top), absolute: true);
                            velocity.Y = -velocity.Y;
                            break;
                        }
                    }

            //Bounds-checking

            if (x >= playfield.X - Width)
            {
                x = playfield.X - Width - 1;
                velocity.X = -velocity.X;
            }
            else if (x < 0)
            {
                x = 0;
                velocity.X = -velocity.X;
            }
            else if (y >= playfield.Y - Height)
            {
                y = playfield.Y - Height - 1;
                velocity.Y = -velocity.Y;
            }
            else if (y < 0)
            {
                y = 0;
                velocity.Y = -velocity.Y;
            }

            base.Update();
        }
    }
}
