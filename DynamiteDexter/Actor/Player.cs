using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Player : Pedestrian, IOutlined
    {
        public enum Direction { Left, Right, Up, Down }

        private const int PLACEMENT_DISTANCE = Game1.TILE_SIZE;

        private static readonly Rectangle[] _playerHitBox =
        {
            new Rectangle(4, 2, Game1.TILE_SIZE - 8, Game1.TILE_SIZE - 3),
            new Rectangle(3, 3, Game1.TILE_SIZE - 6, Game1.TILE_SIZE - 6),
        };

        private readonly OutlineMask _outlineMask;

        private bool placingDynamite;
        private int lives;
        private int dynamiteSticks;
        private int keys;
        private int dollars;
        private int dollarsGoal;
        private Direction direction;

        public OutlineMask TheOutlineMask { get { return _outlineMask; } }
        public int Lives { get { return lives; } }
        public int DynamiteSticks { get { return dynamiteSticks; } }
        public int Keys { get { return keys; } }
        public int Dollars { get { return dollars; } }
        public int DollarsGoal { get { return dollarsGoal; } }

        public Player()
            : base(
                  new SpriteInfo(Images.PLAYER, 112, 112, layer: (int)Game1.Layers.Actor),
                  new CollisionInfo(_playerHitBox, null),
                  new AnimationInfo(3, 4, 5),
                  speed: 3)
        {
            _outlineMask = new OutlineMask(Images.PLAYER_OUTLINE, this);
            lives = 5;
            dynamiteSticks = 10;
            keys = 0;
            dollars = 0;
            dollarsGoal = 100;
            placingDynamite = false;
            direction = Direction.Left;
        }

        public override void MoveLeft()
        {
            direction = Direction.Left;
            base.MoveLeft();
        }
        public override void MoveRight()
        {
            direction = Direction.Right;
            base.MoveRight();
        }
        public override void MoveUp()
        {
            direction = Direction.Up;
            base.MoveUp();
        }
        public override void MoveDown()
        {
            direction = Direction.Down;
            base.MoveDown();
        }

        public void PlaceDynamite()
        {
            if (dynamiteSticks > 0)
                placingDynamite = true;
        }

        public DynamiteIgnited GetDynamiteStick()
        {
            if (placingDynamite)
            {
                placingDynamite = false;

                switch (direction)
                {
                    case Direction.Left: return new DynamiteIgnited(Center.X - (int)Tile.TileSize, Center.Y);
                    case Direction.Right: return new DynamiteIgnited(Center.X + (int)Tile.TileSize, Center.Y);
                    case Direction.Up: return new DynamiteIgnited(Center.X, Center.Y - (int)Tile.TileSize);
                    case Direction.Down: return new DynamiteIgnited(Center.X, Center.Y + (int)Tile.TileSize);
                }
            }

            return null;
        }

        public void IncreaseLives() { lives++; }
        public void DecreaseLives() { lives--; }

        public bool AdjustDollars(int dollars)
        {
            this.dollars += dollars;

            if (this.dollars < 0)
                this.dollars = 0;

            if (this.dollars >= 10000) this.dollars = 999999;

            if (this.dollars >= dollarsGoal)
            {
                dollarsGoal *= 2;

                return true;
            }

            return false;
        }

        public void IncreaseKeys() { keys++; }
        public void DecreaseKeys()
        {
            if (--keys < 0)
                keys = 0;
        }

        public void AdjustDynamiteSticks(int difference)
        {
            dynamiteSticks += difference;

            if (dynamiteSticks < 0)
                dynamiteSticks = 0;
        }

        protected override void SubAnimate()
        {
            if (++delayCount == interval)
            {
                if (++tileSelection.X == sheetDimensions.X)
                    tileSelection.X = 1;

                delayCount = 0;
            }
        }

        public void SyncOutline() { _outlineMask.Sync(x, y, tileSelection.X, tileSelection.Y); }
    }
}
