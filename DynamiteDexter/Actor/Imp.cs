using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Imp : SpriteSheet, IHostile, INavigates, IHunts, IFires
    {
        private const uint MINIMUM_INTERVAL = 120;
        private const uint APPEARANCE_DURATION = 3 * (MINIMUM_INTERVAL / 4);
        private const float PROJECTILE_SPEED = 4.0f;
        private const float DIAGONAL_RATIO = 0.7071f; //Sine or cosine of 45 degrees.

        private static readonly Rectangle[] _impHitBox =
        {
            new Rectangle(1, 1, Game1.TILE_SIZE - 2, 2 * Game1.TILE_SIZE - 2)
        };

        private readonly int _startX;
        private readonly int _startY;
        private readonly IGameObject _target;

        private readonly Vector2 _right = new Vector2(PROJECTILE_SPEED, 0.0f);
        private readonly Vector2 _upRight = new Vector2(DIAGONAL_RATIO * PROJECTILE_SPEED, -DIAGONAL_RATIO * PROJECTILE_SPEED);
        private readonly Vector2 _up = new Vector2(0.0f, -PROJECTILE_SPEED);
        private readonly Vector2 _upLeft = new Vector2(-DIAGONAL_RATIO * PROJECTILE_SPEED, -DIAGONAL_RATIO * PROJECTILE_SPEED);
        private readonly Vector2 _left = new Vector2(-PROJECTILE_SPEED, 0.0f);
        private readonly Vector2 _downLeft = new Vector2(-DIAGONAL_RATIO * PROJECTILE_SPEED, DIAGONAL_RATIO * PROJECTILE_SPEED);
        private readonly Vector2 _down = new Vector2(0.0f, PROJECTILE_SPEED);
        private readonly Vector2 _downRight = new Vector2(DIAGONAL_RATIO * PROJECTILE_SPEED, DIAGONAL_RATIO * PROJECTILE_SPEED);

        private uint count;
        private IProjectile[] chamber;
        private List<Terrain> terrain;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public IGameObject Target { get { return _target; } }
        public List<Terrain> Terrain { get { return terrain; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public bool Lethal
        {
            get
            {
                return count >= (uint)(0.67 * APPEARANCE_DURATION) && count < (uint)(0.78 * APPEARANCE_DURATION);
            }
        }

        public Imp(IGameObject target)
            : base(
                new SpriteInfo(Images.IMP, 0, 0, (int)Game1.Layers.Actor),
                new CollisionInfo(_impHitBox, null),
                new AnimationInfo(5, 1, 0))
        {
            _startX = 0;
            _startY = 0;
            _target = target;
            count = 1;
        }

        public void EmptyChamber() { chamber = null; }

        public void AcquireSet(List<IGameObject> spriteSet)
        {
            terrain = new List<Terrain>();

            foreach (IGameObject i in spriteSet)
                if (i is Terrain)
                    terrain.Add(i as Terrain);
        }

        public void Reset()
        {
            count = 1;
            x = _startX;
            y = _startY;
        }

        protected override void Animate()
        {
            if (count <= (uint)(0.033 * APPEARANCE_DURATION))
            {
                tileSelection.X = 4;
                render = true;
            }
            else if (count <= (uint)(0.067 * APPEARANCE_DURATION))
            {
                tileSelection.X = 3;
                render = true;
            }
            else if (count <= (uint)(0.67 * APPEARANCE_DURATION))
            {
                tileSelection.X = 2;
                render = true;
            }
            else if (count <= (uint)(0.78 * APPEARANCE_DURATION))
            {
                tileSelection.X = 2;
                render = !render;
            }
            else if (count <= (uint)(0.89 * APPEARANCE_DURATION))
            {
                tileSelection.X = 1;
                render = !render;
            }
            else if (count <= (uint)(1.00 * APPEARANCE_DURATION))
            {
                tileSelection.X = 0;
                render = !render;
            }
            else render = false;

            SetFrame();
        }

        public override void Update()
        {
            if (count == MINIMUM_INTERVAL / 4)
            {
                chamber = new IProjectile[8];

                chamber[0] = new Bullet(Center.X, Center.Y, new Vector2(PROJECTILE_SPEED, 0.0f), this, true);
                chamber[1] = new Bullet(Center.X, Center.Y, new Vector2(DIAGONAL_RATIO * PROJECTILE_SPEED, -DIAGONAL_RATIO * PROJECTILE_SPEED), this, true);
                chamber[2] = new Bullet(Center.X, Center.Y, new Vector2(0.0f, -PROJECTILE_SPEED), this, true);
                chamber[3] = new Bullet(Center.X, Center.Y, new Vector2(-DIAGONAL_RATIO * PROJECTILE_SPEED, -DIAGONAL_RATIO * PROJECTILE_SPEED), this, true);
                chamber[4] = new Bullet(Center.X, Center.Y, new Vector2(-PROJECTILE_SPEED, 0.0f), this, true);
                chamber[5] = new Bullet(Center.X, Center.Y, new Vector2(-DIAGONAL_RATIO * PROJECTILE_SPEED, DIAGONAL_RATIO * PROJECTILE_SPEED), this, true);
                chamber[6] = new Bullet(Center.X, Center.Y, new Vector2(0.0f, PROJECTILE_SPEED), this, true);
                chamber[7] = new Bullet(Center.X, Center.Y, new Vector2(DIAGONAL_RATIO * PROJECTILE_SPEED, DIAGONAL_RATIO * PROJECTILE_SPEED), this, true);
            }

            if (--count == 0)
            {
                count = (uint)Game1.rand.Next((int)MINIMUM_INTERVAL, 2 * (int)MINIMUM_INTERVAL);

                Point[] positionCandidates = new Point[4];

                for (int i = 0; i < positionCandidates.Length; i++)
                {
                    bool collides;
                    Rectangle tentativeRect;

                    do
                    {
                        collides = false;
                        tentativeRect = new Rectangle(
                            Game1.TILE_SIZE * Game1.rand.Next(Game1.GRID_SIZE_X),
                            Game1.TILE_SIZE * Game1.rand.Next(Game1.GRID_SIZE_Y),
                            Game1.TILE_SIZE,
                            Game1.TILE_SIZE * 2);

                        foreach (Terrain j in terrain)

                            if (tentativeRect.Intersects(j.Rect))
                            {
                                collides = true;
                                break;
                            }
                    } while (collides);

                    positionCandidates[i] = new Point(tentativeRect.X, tentativeRect.Y);
                }

                float shortestDistance = 10000; //Arbitrarily high number.
                Point closestPosition = positionCandidates[0];

                for (int i = 0; i < positionCandidates.Length; i++)
                {
                    float a = _target.X - positionCandidates[i].X;
                    float b = _target.Y - positionCandidates[i].Y;
                    float c = (float)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                    if (c < shortestDistance)
                    {
                        shortestDistance = c;
                        closestPosition = positionCandidates[i];
                    }
                }

                x = closestPosition.X;
                y = closestPosition.Y;
            }

            base.Update();
        }
    }
}
