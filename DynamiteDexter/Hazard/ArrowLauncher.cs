using System;
using System.Collections.Generic;

namespace DynamiteDexter
{
    public class ArrowLauncher : Terrain, IFires, IHasAttachments
    {
        private const int NUM_QUARTERS = 4;

        private readonly bool _upward;
        private readonly Game1.SyncStates _syncState;

        private IProjectile[] chamber;

        public readonly Terrain Rear;

        public IProjectile[] Chamber { get { return chamber; } }
        public List<IGameObject> Attachments
        {
            get
            {
                List<IGameObject> attachment = new List<IGameObject>();
                attachment.Add(Rear);

                return attachment;
            }
        }

        public ArrowLauncher(int x, int y, bool upward, Game1.SyncStates syncState)
            : base(upward ? Images.ARROW_LAUNCHER_UPWARD_TOP : Images.ARROW_LAUNCHER_DOWNWARD_BOTTOM, x, y)
        {
            _upward = upward;
            _syncState = syncState;

            Rear = new Terrain(
                upward ? Images.ARROW_LAUNCHER_UPWARD_BOTTOM : Images.ARROW_LAUNCHER_DOWNWARD_TOP,
                x,
                upward ? y + 1 : y - 1);
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
                chamber[0] = new Arrow(Center.X, _upward ? Top : Bottom, _upward ? true : false, this);
            }
        }
    }
}
