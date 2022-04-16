using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class SuperComputer : SpriteSheet, IBoss, IHasAttachments
    {
        private const int START_X = 1 * Game1.TILE_SIZE;
        private const int START_Y = 1 * Game1.TILE_SIZE;
        private const int NUM_TERMINALS = 5;

        public static Rectangle[] _superComputerHitBox =
        {
            new Rectangle(0, 0, 2 * Game1.TILE_SIZE, 2 * Game1.TILE_SIZE)
        };
        private static readonly Sprite _appearingForm = new Sprite(
            Images.APPEARING_SUPER_COMPUTER,
            START_X,
            START_Y,
            (int)Game1.Layers.Terrain);

        private readonly int _startX;
        private readonly int _startY;
        private readonly Terminal[] _terminal;
        private readonly Laser[] _laser;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public int Vitality { get { return remove ? 0 : 1; } }
        public bool Flashing { get { return false; } }
        public Rectangle HitBoxAssault { get { return GetHitBox(0); } }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachments = new List<IGameObject>();
                attachments.AddRange(_terminal);
                attachments.AddRange(_laser);

                return attachments;
            }
        }

        public SuperComputer()
            : base(
                  new SpriteInfo(Images.SUPER_COMPUTER, START_X, START_Y, (int)Game1.Layers.Terrain),
                  new SpriteExtraInfo(null, Color.White, SpriteEffects.None),
                  new CollisionInfo(_superComputerHitBox, null),
                  new AnimationInfo(8, 1, 4))
        {
            _startX = START_X;
            _startY = START_Y;
            _terminal = new Terminal[NUM_TERMINALS]
            {
                new Terminal(this),
                new Terminal(this),
                new Terminal(this),
                new Terminal(this),
                new Terminal(this)
            };
            _laser = new Laser[4 * NUM_TERMINALS]
            {
                new Laser(1, 3, true, false),
                new Laser(2, 3, true, false),
                new Laser(1, 3, true, true),
                new Laser(2, 3, true, true),
                new Laser(1, 4, true, false),
                new Laser(2, 4, true, false),
                new Laser(1, 4, true, true),
                new Laser(2, 4, true, true),
                new Laser(1, 5, true, false),
                new Laser(2, 5, true, false),
                new Laser(1, 5, true, true),
                new Laser(2, 5, true, true),
                new Laser(1, 6, true, false),
                new Laser(2, 6, true, false),
                new Laser(1, 6, true, true),
                new Laser(2, 6, true, true),
                new Laser(1, 7, true, false),
                new Laser(2, 7, true, false),
                new Laser(1, 7, true, true),
                new Laser(2, 7, true, true)
            };
        }

        public void Reset()
        {
            x = START_X;
            y = START_Y;
        }

        public Laser[] ComponentRemoval()
        {
            Laser[] toRemove;

            for (int i = 0; i < NUM_TERMINALS; i++)
            {
                if (_terminal[i].Remove)
                {
                    int startingIndex = i * 4;

                    toRemove = new Laser[4]
                    {
                        _laser[startingIndex + 0],
                        _laser[startingIndex + 1],
                        _laser[startingIndex + 2],
                        _laser[startingIndex + 3]
                    };

                    _terminal[i].Remove = false;

                    return toRemove;
                }                  
            }

            return null;
        }

        public void Strike() { remove = true; }
    }

    public class Terminal : Breakable, IFires, INavigates
    {
        private enum Directions { Left, Up, Right, Down, EnumSize }

        private const int MAINFRAME_BOUNDARY_X = 4;
        private const int MAINFRAME_BOUNDARY_Y = 7;
        private const int PROJECTILE_SPEED = 5;
        private const uint FIRE_INTERVAL = 5;
        private const uint SEQUENCE_DURATION = 180;
        private const uint MARKER_FIRE = SEQUENCE_DURATION / 2;
        private const uint MARKER_RETREAT = MARKER_FIRE + 30;
        private const uint NUM_OPENING_FRAMES = 16;

        private readonly SuperComputer _parent;

        private uint count;
        private Directions currentDirection;
        private IProjectile[] chamber;
        private List<IGameObject> spriteSet;

        public IProjectile[] Chamber { get { return chamber; } }

        public Terminal(SuperComputer parent)
            : base(Images.TERMINAL, 0, 0, 20, 2)
        {
            _parent = parent;
            currentDirection = (Directions)Game1.rand.Next((int)Directions.EnumSize);
            count = 0;

            ChangeLocation();
        }

        public void EmptyChamber() { chamber = null; }

        public void AcquireSet(List<IGameObject> spriteSet) { this.spriteSet = spriteSet; }

        public void Destroy() { remove = true; }

        protected override void Animate()
        {
            if (count < MARKER_FIRE - 30)
            {
                render = true;

                if (++delayCount == interval)
                {
                    delayCount = 0;

                    if (tileSelection.X < NUM_OPENING_FRAMES - 1)
                        tileSelection.X++;
                    else if (++tileSelection.X == sheetDimensions.X)
                            tileSelection.X = (int)NUM_OPENING_FRAMES;
                }
            }
            else if (count < MARKER_RETREAT)

                tileSelection.X = (int)NUM_OPENING_FRAMES + (int)currentDirection;

            else //if (count >= MARKER_RETREAT)
            {
                if (tileSelection.X >= NUM_OPENING_FRAMES)
                    tileSelection.X = (int)NUM_OPENING_FRAMES - 1;

                if (++delayCount == interval)
                {
                    delayCount = 0;

                    if (tileSelection.X > 0)
                        tileSelection.X--;
                    else render = false;
                }
            }

            SetFrame();
        }

        private void ChangeLocation()
        {
            Rectangle check;
            bool intersectionFound;

            do
            {
                intersectionFound = false;

                do
                {
                    check = new Rectangle(
                        Game1.rand.Next(1, Game1.GRID_SIZE_X - 1) * Game1.TILE_SIZE,
                        Game1.rand.Next(1, Game1.GRID_SIZE_X - 1) * Game1.TILE_SIZE,
                        Game1.TILE_SIZE,
                        Game1.TILE_SIZE);
                } while (check.X <= MAINFRAME_BOUNDARY_X * Game1.TILE_SIZE && check.Y <= MAINFRAME_BOUNDARY_Y * Game1.TILE_SIZE);

                if (spriteSet != null)

                    foreach (IGameObject i in spriteSet)

                        if ((i is Terrain || i is Terminal || i is Laser || i is SuperComputer) && check.Intersects(i.Rect))
                        {
                            intersectionFound = true;
                            break;
                        }
            }
            while (intersectionFound);

            Reposition(check.X / Game1.TILE_SIZE, check.Y / Game1.TILE_SIZE);
            currentDirection = (Directions)Game1.rand.Next((int)Directions.EnumSize);

            //Check quadrant and have direction tend toward the center of the room.

            const int PROBABILITY = 8;

            if (gridX <= (Game1.GRID_SIZE_X  / 2) && gridY <= (Game1.GRID_SIZE_Y / 2))
            {
                if (currentDirection == Directions.Left && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Right;
                else if (currentDirection == Directions.Up && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Down;
            }
            else if (gridX > (Game1.GRID_SIZE_X / 2) && gridY <= (Game1.GRID_SIZE_Y / 2))
            {
                if (currentDirection == Directions.Right && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Left;
                else if (currentDirection == Directions.Up && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Down;
            }
            else if (gridX <= (Game1.GRID_SIZE_X / 2) && gridY > (Game1.GRID_SIZE_Y / 2))
            {
                if (currentDirection == Directions.Left && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Right;
                else if (currentDirection == Directions.Down && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Up;
            }
            else //if (gridX >= (Game1.GRID_SIZE_X / 2) && gridY <= (Game1.GRID_SIZE_Y / 2))
            {
                if (currentDirection == Directions.Right && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Left;
                else if (currentDirection == Directions.Down && Game1.rand.Next(PROBABILITY) > 0)
                    currentDirection = Directions.Up;
            }
        }

        public override void Update()
        {
            if (++count == SEQUENCE_DURATION)
            {
                count = 0;
                ChangeLocation();
            }
            else
            {
                int NUM_ROUNDS = 5;

                for (int i = 0; i < NUM_ROUNDS; i++)

                    if (count == MARKER_FIRE + i * FIRE_INTERVAL)
                    {
                        chamber = new IProjectile[1];

                        switch (currentDirection)
                        {
                            case Directions.Left:
                                chamber[0] = new Bullet(Left, Center.Y, new Vector2(-PROJECTILE_SPEED, 0.0f), this, true);
                                break;
                            case Directions.Up:
                                chamber[0] = new Bullet(Center.X, Top, new Vector2(0.0f, -PROJECTILE_SPEED), this, true);
                                break;
                            case Directions.Right:
                                chamber[0] = new Bullet(Right - 1, Center.Y, new Vector2(PROJECTILE_SPEED, 0.0f), this, true);
                                break;
                            case Directions.Down:
                                chamber[0] = new Bullet(Center.X, Bottom - 1, new Vector2(0.0f, PROJECTILE_SPEED), this, true);
                                break;
                        }
                    }
            }

            base.Update();
        }
    }
}
