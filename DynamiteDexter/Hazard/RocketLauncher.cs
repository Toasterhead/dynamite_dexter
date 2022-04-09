using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class RocketLauncher : Terrain, IFires
    {
        private const int NUM_QUARTERS = 4;

        private readonly bool _leftward;
        private readonly Game1.SyncStates _syncState;

        private IProjectile[] chamber;

        public IProjectile[] Chamber { get { return chamber; } }

        public RocketLauncher(int x, int y, bool leftward, Game1.SyncStates syncState)
            : base(leftward ? Images.MISSILE_LAUNCHER_LEFT : Images.MISSILE_LAUNCHER_RIGHT, x, y)
        {
            _leftward = leftward;
            _syncState = syncState;
        }

        public void EmptyChamber() { chamber = null; }

        public override void Update()
        {
            bool willFire = false;
            int count = Game1.universalTimer % (NUM_QUARTERS * Game1.SYNC_QUARTER);

            if (_syncState == Game1.SyncStates.FirstAndThirdQuarter)

                willFire = count == 0 * Game1.SYNC_QUARTER || count == 2 * Game1.SYNC_QUARTER;

            else if (_syncState == Game1.SyncStates.SecondAndFourthQuarter)

                willFire = count == 1 * Game1.SYNC_QUARTER || count == 3 * Game1.SYNC_QUARTER;

            else willFire = count == (int)_syncState * Game1.SYNC_QUARTER;

            if (willFire)
            {
                chamber = new IProjectile[1];
                chamber[0] = new Rocket(_leftward ? Left : Right, Center.Y, _leftward, true, this);
            }
        }
    }
}
