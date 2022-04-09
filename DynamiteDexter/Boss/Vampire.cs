using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Vampire : SpriteSheet, IBoss, IFires
    {
        private const int START_X = (Game1.GRID_SIZE_X / 2) * Game1.TILE_SIZE;
        private const int START_Y = (Game1.GRID_SIZE_Y / 2) * Game1.TILE_SIZE;
        private const int SPEED = 3;
        private const int PROJECTILE_SPEED = 5;
        private const int THRESHOLD = Game1.TILE_SIZE / 4;
        private const int INITIAL_VITALITY = 3;
        private const uint FLASH_DURATION = 30;
        private const uint INTERVAL = 90;
        private const uint CAPE_OPEN_MARK = 75;
        private const uint RELEASE_MARK = 60;
        private const float DIAGONAL_RATIO = 0.7071f; //Adjacent divided by hypotenuse at 45 degrees.

        private static readonly Rectangle[] _vampireHitBox =
        {
            new Rectangle(Game1.TILE_SIZE / 2, 2, Game1.TILE_SIZE, Game1.TILE_SIZE),
            new Rectangle(3 * (Game1.TILE_SIZE / 4), 1, Game1.TILE_SIZE / 2, (Game1.TILE_SIZE * 2) - 2)
        };
        private static readonly Sprite _appearingForm = new Sprite(
            Images.APPEARING_VAMPIRE,
            START_X,
            START_Y,
            (int)Game1.Layers.Actor);

        private readonly int _startX;
        private readonly int _startY;

        private int vitality;
        private uint flashCount;
        private uint? count;
        private Point targetPosition;
        private IProjectile[] chamber;

        public int StartX { get { return START_X; } }
        public int StartY { get { return START_Y; } }
        public int Vitality { get { return vitality; } }
        public uint FlashCount { get { return flashCount; } }
        public bool Flashing { get { return flashCount % 2 != 0; } }
        public Sprite AppearingForm { get { return _appearingForm; } }
        public IProjectile[] Chamber { get { return chamber; } }
        public Rectangle HitBoxAssault
        {
            get
            {
                if (count == null)

                    return GetHitBox(0);

                return GetHitBox(1);
            }
        }

        public Vampire()
            : base(
                  new SpriteInfo(Images.VAMPIRE, START_X, START_Y, (int)Game1.Layers.Actor),
                  new CollisionInfo(_vampireHitBox, null),
                  new AnimationInfo(4, 3, 5))
        {
            _startX = START_X;
            _startY = START_Y;
            vitality = INITIAL_VITALITY;
            flashCount = 0;
            count = INTERVAL;

            tileSelection.X = 0;
            tileSelection.Y = 2;

            targetPosition = new Point(
                                    Game1.rand.Next(1, Game1.playfield.X / Game1.TILE_SIZE - 2),
                                    Game1.rand.Next(1, Game1.playfield.X / Game1.TILE_SIZE - 2));
        }

        public void Reset()
        {
            x = _startX;
            y = _startY;
            vitality = INITIAL_VITALITY;
            flashCount = 0;
        }

        public void Strike()
        {
            if (--vitality == 0)
                remove = true;

            flashCount = FLASH_DURATION;
        }

        public void EmptyChamber() { chamber = null; }

        protected override void Animate()
        {
            if (count == null && ++delayCount >= interval)
            {
                delayCount = 0;
                tileSelection.X = tileSelection.X == 0 ? 1 : 0;
                tileSelection.Y = 0;
            }
            else if (count != null)
            {
                uint halfInterval = interval / 2;

                if (count == INTERVAL)
                {
                    delayCount = 0;
                    tileSelection.X = 0;
                    tileSelection.Y = 1;
                }
                else if (count > INTERVAL / 2 && tileSelection.Y == 1 && ++delayCount >= interval)
                {
                    delayCount = 0;

                    if (++tileSelection.X >= sheetDimensions.X)
                    {
                        tileSelection.X = 0;
                        tileSelection.Y = 2;
                    }
                }
                else if (count > (sheetDimensions.X + 2) * halfInterval && count <= 5 * (INTERVAL / 8) && tileSelection.Y == 2 && ++delayCount >= interval)
                {
                    delayCount = 0;

                    if (++tileSelection.X >= sheetDimensions.X)
                        tileSelection.X = sheetDimensions.X - 1;
                }
                else if (count <= (sheetDimensions.X + 2) * halfInterval && tileSelection.Y == 2 && ++delayCount >= halfInterval)
                {
                    delayCount = 0;

                    if (--tileSelection.X < 0)
                    {
                        tileSelection.X = sheetDimensions.X - 1;
                        tileSelection.Y = 1;
                    }
                }
                else if (count <= (sheetDimensions.X + 2) * halfInterval && tileSelection.Y == 1 && ++delayCount >= halfInterval)
                {
                    delayCount = 0;

                    if (--tileSelection.X < 0)
                        tileSelection.X = 0;
                }
            }

            SetFrame();
        }

        public override void Update()
        {
            if (count != null)
            {
                if (--count == 0)
                    count = null;
                else if (count == INTERVAL / 2)
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
            }
            else
            {
                Point targetPositionByPixel = new Point(targetPosition.X * Game1.TILE_SIZE, targetPosition.Y * Game1.TILE_SIZE);

                velocity = new Vector2();

                if (x <= targetPositionByPixel.X + THRESHOLD &&
                    x >= targetPositionByPixel.X - THRESHOLD &&
                    y <= targetPositionByPixel.Y + THRESHOLD &&
                    y >= targetPositionByPixel.Y - THRESHOLD)
                {
                    x = targetPositionByPixel.X;
                    y = targetPositionByPixel.Y;

                    targetPosition = new Point(
                                    Game1.rand.Next(1, Game1.playfield.X / Game1.TILE_SIZE - 2),
                                    Game1.rand.Next(1, Game1.playfield.X / Game1.TILE_SIZE - 2));
                    count = INTERVAL;
                }
                else
                {
                    if (targetPositionByPixel.X < x - THRESHOLD)
                        velocity.X = -SPEED;
                    else if (targetPositionByPixel.X > x + THRESHOLD)
                        velocity.X = SPEED;

                    if (targetPositionByPixel.Y < y - THRESHOLD)
                        velocity.Y = -SPEED;
                    else if (targetPositionByPixel.Y > y + THRESHOLD)
                        velocity.Y = SPEED;

                    if (velocity.X != 0.0f && velocity.Y != 0.0f)
                        velocity *= DIAGONAL_RATIO;
                }
            }

            if (flashCount > 0)
                flashCount--;

            if (flashCount > 0 && flashCount % 2 == 0)
                render = false;
            else render = true;

            base.Update();
        }
    }
}
