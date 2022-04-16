using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Impaler : SpriteSheet, IHostile
    {
        private const int SPIKE_LENGTH = 2 * Game1.TILE_SIZE;
        private const int NUM_QUARTERS = 4;
        private const uint NUM_POSITIONS = 9;

        private static readonly Rectangle[] _impalerHitBox =
        {
            new Rectangle(6, 8, 4, 0),
            new Rectangle(6, 8, 4, 8),
            new Rectangle(6, 8, 4, 12),
            new Rectangle(6, 8, 4, 16),
            new Rectangle(6, 8, 4, 20),
            new Rectangle(6, 8, 4, 24),
            new Rectangle(6, 8, 4, 28),
            new Rectangle(6, 8, 4, 32),
            new Rectangle(6, 8, 4, 36),
            new Rectangle(6, 8, 4, 40),
        };

        private readonly bool _upward;
        private readonly int _startX;
        private readonly int _startY;
        private readonly Game1.SyncStates _syncState;
        private readonly Terrain _source;

        private uint outwardPosition;

        public int StartX { get { return _startX; } }
        public int StartY { get { return _startY; } }
        public Rectangle HitBoxAssault
        {
            get
            {
                Rectangle hitBoxAssault;

                if (outwardPosition >= NUM_POSITIONS)
                {
                    hitBoxAssault = GetHitBox(0);

                    if (_upward)
                        hitBoxAssault = new Rectangle(
                            hitBoxAssault.X, 
                            hitBoxAssault.Y + SPIKE_LENGTH, 
                            hitBoxAssault.Width, 
                            hitBoxAssault.Height);

                    return hitBoxAssault;
                }
                else hitBoxAssault = GetHitBox((int)outwardPosition, absolute: true);

                if (_upward)
                {
                    int posY = Bottom - Game1.TILE_SIZE + 4 - (4 * (int)outwardPosition);

                    return new Rectangle(hitBoxAssault.X, posY, hitBoxAssault.Width, hitBoxAssault.Height);
                }

                return hitBoxAssault;
            }
        }
        public Terrain Source { get { return _source; } }

        public Impaler(Terrain source, bool upward, Game1.SyncStates syncState)
            : base(
                  new SpriteInfo(
                      Images.IMPALER, 
                      source.X, 
                      upward ? source.Y - SPIKE_LENGTH : source.Y, 
                      (int)Game1.Layers.Actor),
                  new CollisionInfo(_impalerHitBox, null),
                  new AnimationInfo(9, 2, 0))
        {
            _upward = upward;
            _syncState = syncState;
            _source = source;
            _startX = X;
            _startY = Y;

            outwardPosition = 0;

            if (upward)
                tileSelection.Y = 1;
        }

        public void Reset() { }

        protected override void Animate()
        {
            if (outwardPosition < NUM_POSITIONS)
            {
                render = true;
                tileSelection.X = (int)outwardPosition;
            }
            else render = false;

            SetFrame();
        }

        public override void Update()
        {
            int count = Game1.universalTimer % (NUM_QUARTERS * Game1.SYNC_QUARTER);

            if (_syncState == Game1.SyncStates.SecondQuarter)
                count =
                    count >= Game1.SYNC_QUARTER ?
                    count - Game1.SYNC_QUARTER :
                    int.MaxValue;
            else if (_syncState == Game1.SyncStates.ThirdQuarter)
                count =
                    count >= 2 * Game1.SYNC_QUARTER ?
                    count - 2 * Game1.SYNC_QUARTER :
                    int.MaxValue;
            else if (_syncState == Game1.SyncStates.FourthQuarter)
                count =
                    count >= 3 * Game1.SYNC_QUARTER ?
                    count - 3 * Game1.SYNC_QUARTER :
                    (count < Game1.SYNC_QUARTER ? Game1.SYNC_QUARTER + count : int.MaxValue);
            else if (_syncState == Game1.SyncStates.FirstAndThirdQuarter)
                count %= 2 * Game1.SYNC_QUARTER;
            else if (_syncState == Game1.SyncStates.SecondAndFourthQuarter)
            {
                if (count >= Game1.SYNC_QUARTER && count < 3 * Game1.SYNC_QUARTER)
                    count -= Game1.SYNC_QUARTER;
                else if (count >= 3 * Game1.SYNC_QUARTER)
                    count -= 3 * Game1.SYNC_QUARTER;
                else count = Game1.SYNC_QUARTER + count;
            }

            if (count < NUM_POSITIONS)
                outwardPosition = (uint)count;
            else if (count < 2 * NUM_POSITIONS)
                outwardPosition = (uint)((NUM_POSITIONS - 1) - (count % NUM_POSITIONS));
            else outwardPosition = NUM_POSITIONS;

            base.Update();
        }
    }
}
